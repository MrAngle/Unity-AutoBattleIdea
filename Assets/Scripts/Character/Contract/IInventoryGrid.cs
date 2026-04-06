using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface IInventoryGrid {
        bool canPlace(ShapeArchetype data, Vector2Int origin);
        void place(ShapeArchetype data, Vector2Int origin);
        void remove(ShapeArchetype data, Vector2Int origin);
        int getWidthCellsNumber();

        int getHeightCellsNumber();

        CellState getState(Vector2Int coord);
    }
}