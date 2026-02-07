using System.Runtime.CompilerServices;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal class InventoryFactoryService : IInventoryFactory {
        private readonly SignalBus signalBus;
        private readonly IItemFactory itemFactory;

        [Inject]
        public InventoryFactoryService(IItemFactory itemFactory, SignalBus signalBus) {
            this.signalBus = NullGuard.NotNullOrThrow(signalBus);
            this.itemFactory = NullGuard.NotNullOrThrow(itemFactory);
        }

        public ICharacterInventory createCharacterInventory() {
            return InventoryAggregate.create(itemFactory, signalBus);
        }
    }
}