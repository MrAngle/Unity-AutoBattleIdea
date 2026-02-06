using MageFactory.Inventory.Api;
using Zenject;

namespace MageFactory.Inventory.Domain.Service {
    public class InventoryAggregateFactoryService : IInventoryAggregateFactory {
        private readonly SignalBus _signalBus;

        [Inject]
        public InventoryAggregateFactoryService(
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            _signalBus = signalBus;
        }

        public ICharacterInventoryFacade CreateCharacterInventory() {
            return InventoryAggregate.create(_signalBus);
        }
    }
}