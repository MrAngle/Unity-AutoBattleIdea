using System.Collections.Generic;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Shared.Contract {
    public interface IGridItemPlaced {
        long getId();
        Vector2Int getOrigin();
        IReadOnlyCollection<Vector2Int> getOccupiedCells();
        ShapeArchetype getShape();
    }
}