using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface IInventoryGrid {
        int Width { get; }
        int Height { get; }
        CellState getState(Vector2Int coord);

        bool canPlace(ShapeArchetype data, Vector2Int origin);
        void place(ShapeArchetype data, Vector2Int origin);
    }
}