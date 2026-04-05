using MageFactory.ActionEffect;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlaceableItem {
        IInventoryPlacedItem toPlacedItem(
            IInventoryPosition inventoryPosition);

        public IItemDefinition getItemDefinition();
    }
}