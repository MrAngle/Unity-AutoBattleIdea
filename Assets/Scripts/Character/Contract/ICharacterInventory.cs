using System.Collections.Generic;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventory {
        IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();
        IInventoryGrid getInventoryGrid();

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);

        // public ICharacterEquippedItem place(ICharacterEquipableItem placeableItem, Vector2Int origin);
        public bool canPlace(PlaceItemQuery placeItemCommand);
        // public bool canPlace(ICharacterEquipableItem placeableItem, Vector2Int origin);
    }
}