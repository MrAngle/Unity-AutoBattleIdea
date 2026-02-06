using MageFactory.Inventory.Api;
using Zenject;

namespace MageFactory.Inventory.Domain.Service {
    public class InventoryAggregateFactoryService : IInventoryAggregateFactory {
        private readonly IEntryPointFactory _entryPointFactory;
        private readonly SignalBus _signalBus;

        [Inject]
        public InventoryAggregateFactoryService(
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