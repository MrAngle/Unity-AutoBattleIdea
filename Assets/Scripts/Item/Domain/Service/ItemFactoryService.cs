using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Shared.Contract;
using Zenject;
// using MageFactory.Flow.Api;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Item.Domain.Service {
    internal class ItemFactoryService : IEntryPointFactory, IItemFactory {
        // private readonly IFlowFactory _flowFactory;

        [Inject]
        internal ItemFactoryService( /*IFlowFactory flowFactory*/) {
            // _flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }

        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities
        ) {
            var placedEntryPoint = PlacedEntryPoint.create(entryPointArchetype, inventoryPosition /*, _flowFactory,
                characterCombatCapabilities*/);

            return placedEntryPoint;
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
            return TickEntryPoint.create(itemDefinition, this);
        }
    }
}