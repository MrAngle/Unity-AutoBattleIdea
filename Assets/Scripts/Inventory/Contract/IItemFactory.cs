using MageFactory.Inventory.Contract.Dto;

namespace MageFactory.Inventory.Contract {
    public interface IItemFactory {
        IInventoryPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand);
        // IInventoryPlacedItem createPlacedItem(CreatePlaceableItemCommand createPlaceableItemCommand);
    }
}