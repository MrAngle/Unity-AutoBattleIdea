using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlaceableItem {
        IInventoryPlacedItem toPlacedItem(IInventoryInspector gridInspector /*do zastanowienia czy potrzebne*/,
            Vector2Int origin);

        ShapeArchetype getShape();
    }
}