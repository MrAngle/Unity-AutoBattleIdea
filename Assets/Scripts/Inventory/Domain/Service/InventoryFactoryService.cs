using System.Runtime.CompilerServices;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal class InventoryFactoryService : IInventoryFactory {
        private readonly SignalBus signalBus;

        [Inject]
        public InventoryFactoryService(SignalBus signalBus) {
            this.signalBus = NullGuard.NotNullOrThrow(signalBus);
        }

        public ICharacterInventoryFacade CreateCharacterInventory() {
            return InventoryAggregate.create(signalBus);
        }
    }
}