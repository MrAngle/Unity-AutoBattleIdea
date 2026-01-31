using Contracts.Inventory;
using Inventory.EntryPoints;
using Zenject;

namespace Inventory {
    public interface IInventoryAggregateFactory {
        ICharacterInventoryFacade CreateCharacterInventory();
    }


    public class InventoryAggregateFactory : IInventoryAggregateFactory {
        private readonly IEntryPointFactory _entryPointFactory;
        private readonly SignalBus _signalBus;

        [Inject]
        public InventoryAggregateFactory(
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            _signalBus = signalBus;
            _entryPointFactory = entryPointFactory;
        }


        // public InventoryAggregate Create() {
        //     var inventoryAggregate = InventoryAggregate.Create(_signalBus, _entryPointFactory);
        //
        //     EntryPointArchetype entryPointArchetype =
        //         _entryPointFactory.CreateArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
        //         // EntryPointArchetype.Create(FlowKind.Damage, ShapeCatalog.Square1x1, _entryPointFactory);
        //
        //     inventoryAggregate.Place(entryPointArchetype, new Vector2Int(0, 0)); // for now
        //
        //     return inventoryAggregate;
        // }

        public ICharacterInventoryFacade CreateCharacterInventory() {
            return InventoryAggregate.Create(_signalBus, _entryPointFactory);
        }
    }
}