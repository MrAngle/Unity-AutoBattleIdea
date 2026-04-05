using System.Runtime.CompilerServices;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Item.Domain.InventoryItems;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Item.Domain.Service {
    internal class ItemFactoryService : IEntryPointFactory, IItemFactory {
        [Inject]
        internal ItemFactoryService() {
        }

        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
        ) {
            var placedEntryPoint = EntryPointItem.create(entryPointArchetype, inventoryPosition);

            return new InventoryPlacedEntryPoint(placedEntryPoint);
        }

        public IInventoryPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand) {
            if (createPlaceableItemCommand.itemDefinition is IEntryPointDefinition entryPointDefinition) {
                return createPlacableItem(entryPointDefinition);
            }

            return createPlacableItem(createPlaceableItemCommand.itemDefinition);
        }

        private IInventoryPlaceableItem createPlacableItem(IItemDefinition itemDefinition) {
            return ItemArchetype.create(itemDefinition);
        }

        private IInventoryPlaceableItem createPlacableItem(IEntryPointDefinition itemDefinition) {
            return TickEntryPoint.create(itemDefinition, this);
        }
    }
}