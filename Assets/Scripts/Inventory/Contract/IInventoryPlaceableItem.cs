using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlaceableItem {
        IInventoryPlacedItem toPlacedItem(
            // IInventoryInspector gridInspector /*do zastanowienia czy potrzebne*/,
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities
            /*Vector2Int origin*/);

        ShapeArchetype getShape();
    }
}