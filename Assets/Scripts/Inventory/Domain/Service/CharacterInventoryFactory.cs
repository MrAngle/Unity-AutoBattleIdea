using System.Runtime.CompilerServices;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal class CharacterInventoryFactory : ICharacterInventoryFactory {
        private readonly IItemFactory itemFactory;
        private readonly IInventoryEventPublisher inventoryEventPublisher;

        [Inject]
        public CharacterInventoryFactory(IItemFactory itemFactory, IInventoryEventPublisher inventoryEventPublisher) {
            this.itemFactory = NullGuard.NotNullOrThrow(itemFactory);
            this.inventoryEventPublisher = NullGuard.NotNullOrThrow(inventoryEventPublisher);
        }

        public ICharacterInventory createCharacterInventory() {
            InventoryAggregate inventoryAggregate = InventoryAggregate.create(itemFactory, inventoryEventPublisher);

            return new CharacterInventory(inventoryAggregate);
        }
    }
}