using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IInventoryGrid {
        int Width { get; }
        int Height { get; }
        CellState GetState(Vector2Int coord);

        bool CanPlace(ShapeArchetype data, Vector2Int origin);
        void Place(ShapeArchetype data, Vector2Int origin);
    }
}