using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface IReadOnlyInventoryGrid {
        // bool canPlace(ShapeArchetype data, Vector2Int origin);
        // // void place(ShapeArchetype data, Vector2Int origin);
        // void remove(ShapeArchetype data, Vector2Int origin);
        int getWidthCellsNumber();

        int getHeightCellsNumber();

        CellState getState(Vector2Int coord);
    }
}