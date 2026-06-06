using System;
using System.Runtime.CompilerServices;
using MageFactory.ActionEffect;
using MageFactory.CombatEvents;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Item.Domain.InventoryItems;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Item.Domain.Service {
    internal class ItemFactoryService : IEntryPointFactory, IItemFactory {
        [Inject]
        internal ItemFactoryService() {
        }

        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
        ) {
            IEntryPointArchetype archetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            validateEntryPointHook(archetype.getTriggerKind(), archetype.getCombatHook());

            var placedEntryPoint = EntryPointItem.create(archetype, inventoryPosition);

            return archetype.getTriggerKind() switch {
                EntryPointTriggerKind.CombatTick => new InventoryPlacedEntryPoint(placedEntryPoint),
                EntryPointTriggerKind.CombatEvent => new InventoryPlacedEventEntryPoint(placedEntryPoint),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(entryPointArchetype),
                    archetype.getTriggerKind(),
                    "Unsupported entry point trigger kind.")
            };
        }

        public IInventoryPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand) {
            if (createPlaceableItemCommand.itemDefinition is IEntryPointDefinition entryPointDefinition) {
                return createPlacableItem(entryPointDefinition);
            }

            return createPlacableItem(createPlaceableItemCommand.itemDefinition);
        }

        private IInventoryPlaceableItem createPlacableItem(IItemDefinition itemDefinition) {
            return ItemArchetype.create(itemDefinition);
        }

        private IInventoryPlaceableItem createPlacableItem(IEntryPointDefinition itemDefinition) {
            IEntryPointDefinition entryPointDefinition = NullGuard.NotNullOrThrow(itemDefinition);
            validateEntryPointHook(entryPointDefinition.getTriggerKind(), entryPointDefinition.getCombatHook());

            return entryPointDefinition.getTriggerKind() switch {
                EntryPointTriggerKind.CombatTick => TickEntryPoint.create(entryPointDefinition, this),
                EntryPointTriggerKind.CombatEvent => EventEntryPoint.create(entryPointDefinition, this),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(itemDefinition),
                    entryPointDefinition.getTriggerKind(),
                    "Unsupported entry point trigger kind.")
            };
        }

        private static void validateEntryPointHook(EntryPointTriggerKind triggerKind, ICombatHook combatHook) {
            NullGuard.enumDefinedOrThrow(triggerKind);
            ICombatHook hook = NullGuard.NotNullOrThrow(combatHook);

            bool hasHook = hook.getHookType() != CombatHookType.None;
            if (triggerKind == EntryPointTriggerKind.CombatTick && hasHook) {
                throw new ArgumentException(
                    $"CombatTick entry point cannot declare combat hook '{hook.getPlayerFacingName()}'.");
            }

            if (triggerKind == EntryPointTriggerKind.CombatEvent && !hasHook) {
                throw new ArgumentException("CombatEvent entry point must declare a concrete CombatHook.");
            }
        }
    }
}