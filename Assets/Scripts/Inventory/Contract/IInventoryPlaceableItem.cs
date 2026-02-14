using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlaceableItem {
        IInventoryPlacedItem toPlacedItem(
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities);

        ShapeArchetype getShape();
    }
}