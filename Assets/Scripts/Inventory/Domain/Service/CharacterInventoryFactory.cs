using System;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Domain.CharacterEq;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal class CharacterInventoryFactory : ICharacterInventoryFactory {
        private readonly IItemFactory itemFactory;
        private readonly IInventoryEventPublisher inventoryEventPublisher;
        private readonly InventoryGridConfiguration inventoryGridConfiguration;

        [Inject]
        public CharacterInventoryFactory(IItemFactory itemFactory,
                                         IInventoryEventPublisher inventoryEventPublisher,
                                         InventoryGridConfiguration inventoryGridConfiguration) {
            this.itemFactory = NullGuard.NotNullOrThrow(itemFactory);
            this.inventoryEventPublisher = NullGuard.NotNullOrThrow(inventoryEventPublisher);
            this.inventoryGridConfiguration = NullGuard.NotNullOrThrow(inventoryGridConfiguration);
        }

        public ICharacterInventory createCharacterInventory(GridDimensions gridDimensions) {
            GridDimensions requestedGridDimensions = NullGuard.NotNullOrThrow(gridDimensions);

            if (!inventoryGridConfiguration.canUse(requestedGridDimensions)) {
                GridDimensions maxGridDimensions = inventoryGridConfiguration.getMaxGridDimensions();
                throw new ArgumentOutOfRangeException(
                    nameof(gridDimensions),
                    $"Inventory grid {requestedGridDimensions.getWidth()}x{requestedGridDimensions.getHeight()} " +
                    $"exceeds max configured grid {maxGridDimensions.getWidth()}x{maxGridDimensions.getHeight()}.");
            }

            InventoryAggregate inventoryAggregate = InventoryAggregate.create(
                itemFactory,
                inventoryEventPublisher,
                requestedGridDimensions);

            return new CharacterInventory(inventoryAggregate);
        }
    }
}