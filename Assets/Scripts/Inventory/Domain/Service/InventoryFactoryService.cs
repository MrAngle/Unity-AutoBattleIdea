using System.Runtime.CompilerServices;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal class InventoryFactoryService : IInventoryFactory {
        private readonly IItemFactory itemFactory;
        private readonly IInventoryEventHub inventoryEventHub;

        [Inject]
        public InventoryFactoryService(IItemFactory itemFactory, IInventoryEventHub inventoryEventHub) {
            this.itemFactory = NullGuard.NotNullOrThrow(itemFactory);
            this.inventoryEventHub = NullGuard.NotNullOrThrow(inventoryEventHub);
        }

        public ICharacterInventory createCharacterInventory() {
            return InventoryAggregate.create(itemFactory, inventoryEventHub);
        }
    }
}