using MageFactory.Inventory.Api;
using Zenject;

namespace MageFactory.Inventory.Domain.Service {
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

        public ICharacterInventoryFacade CreateCharacterInventory() {
            return InventoryAggregate.Create(_signalBus, _entryPointFactory);
        }
    }
}