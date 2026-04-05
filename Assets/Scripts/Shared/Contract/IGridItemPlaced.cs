using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Shared.Contract {
    public interface IGridItemPlaced {
        Id<ItemId> getId();
        Vector2Int getOrigin();
        IReadOnlyCollection<Vector2Int> getOccupiedCells();
        ShapeArchetype getShape();
    }
}