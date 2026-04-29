using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.CombatContextRuntime;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.InventoryItems {
    internal class InventoryPlacedEntryPoint : IInventoryPlacedEntryPoint {
        private readonly EntryPointItem entryPointItem;
        private CombatTicks ticksUntilNextTrigger;

        public InventoryPlacedEntryPoint(EntryPointItem entryPointItem) {
            this.entryPointItem = NullGuard.NotNullOrThrow(entryPointItem);
            ticksUntilNextTrigger = entryPointItem.getTriggerInterval();
        }

        public Id<ItemId> getId() {
            return entryPointItem.getId();
        }

        public Vector2Int getOrigin() {
            return entryPointItem.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return entryPointItem.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return entryPointItem.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return entryPointItem.prepareItemActionDescription();
        }

        public void updateItemPosition(IInventoryPosition inventoryPosition) {
            entryPointItem.updateItemPosition(inventoryPosition);
        }

        public FlowKind getFlowKind() {
            return entryPointItem.getFlowKind();
        }

        public void tick(
            CombatTicks combatTicks,
            Id<CharacterId> characterId,
            ICombatCapabilities combatCapabilities
        ) {
            ticksUntilNextTrigger -= combatTicks;

            if (ticksUntilNextTrigger > CombatTicks.ZERO) {
                return;
            }

            CreateFlowCombatCommand createFlowCombatCommand = new CreateFlowCombatCommand(
                characterId,
                getId()
            );

            combatCapabilities.command().dispatch(createFlowCombatCommand);

            ticksUntilNextTrigger += entryPointItem.getTriggerInterval();
        }
    }
}