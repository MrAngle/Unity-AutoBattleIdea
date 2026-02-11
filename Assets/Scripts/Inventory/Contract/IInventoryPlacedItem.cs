using MageFactory.ActionEffect;
using MageFactory.Character.Contract;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedItem : ICharacterEquippedItem {
        // public long getId();
        public IActionDescription prepareItemActionDescription();
        // public ShapeArchetype getShape();

        // public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        // public Vector2Int getOrigin();
    }
}