using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        public IItemActionDescription prepareItemActionDescription();
        public ShapeArchetype GetShape();
    }

    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}