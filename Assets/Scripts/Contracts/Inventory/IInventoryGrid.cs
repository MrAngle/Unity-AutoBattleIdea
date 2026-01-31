using Contracts.Items;
using UnityEngine;

namespace Contracts.Inventory {
    public interface IInventoryGrid {
        int Width { get; }
        int Height { get; }
        CellState GetState(Vector2Int coord);

        bool CanPlace(ShapeArchetype data, Vector2Int origin);
        void Place(ShapeArchetype data, Vector2Int origin);

        void RegisterEntryPoint(IPlacedEntryPoint placedEntryPoint);

        // public static IInventoryGrid CreateInventoryGrid(int width, int height, IPlacedEntryPoint placedEntryPoint = null) {
        //     return new InventoryGrid(width, height, placedEntryPoint);
        // }
    }
}