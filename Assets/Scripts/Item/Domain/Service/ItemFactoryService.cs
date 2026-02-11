using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Api;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Item.Domain.Service {
    internal class ItemFactoryService : IEntryPointFactory, IItemFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        internal ItemFactoryService(IFlowFactory flowFactory) {
            _flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }

        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities
        ) {
            var placedEntryPoint = PlacedEntryPoint.create(entryPointArchetype, inventoryPosition, _flowFactory,
                characterCombatCapabilities);

            return placedEntryPoint;
        }

        public IInventoryPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand) {
            if (createPlaceableItemCommand.itemDefinition is IEntryPointDefinition entryPointDefinition)
                // todo change it
                return createPlacableItem(entryPointDefinition);

            return createPlacableItem(createPlaceableItemCommand.itemDefinition);
            // return ItemArchetype.create(createPlaceableItemCommand);
        }

        public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
            return new TickEntryPoint(kind, shapeArchetype, this);
        }

        private IInventoryPlaceableItem createPlacableItem(IItemDefinition itemDefinition) {
            return ItemArchetype.create(itemDefinition);
        }

        private IInventoryPlaceableItem createPlacableItem(IEntryPointDefinition itemDefinition) {
            return TickEntryPoint.create(itemDefinition, this);
        }
    }
}