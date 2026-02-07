using MageFactory.Inventory.Api;
using MageFactory.Item.Api;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Domain.Service {
    public class InventoryFactoryService : IInventoryFactory {
        private readonly SignalBus signalBus;
        private readonly IEntryPointFactory entryPointFactory;

        [Inject]
        public InventoryFactoryService(
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            this.signalBus = signalBus;
            this.entryPointFactory = entryPointFactory;
        }

        public ICharacterInventoryFacade CreateCharacterInventory() {
            return InventoryAggregate.create(signalBus);
        }

        public IPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector) {
            return entryPointFactory.CreatePlacedEntryPoint(archetype, position, gridInspector);
        }

        // @Override
        // public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
        //     return entryPointFactory.CreateArchetypeEntryPoint(kind, shapeArchetype);
        // }
    }
}