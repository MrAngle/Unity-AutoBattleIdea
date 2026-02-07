using System.Collections.Generic;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedItem {
        public long getId();
        public Vector2Int getOrigin();
        public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        public ShapeArchetype getShape();
    }

    // public interface IPlacedItem : IInventoryPosition {
    //     public long getId();
    //     public IActionDescription prepareItemActionDescription();
    //     public ShapeArchetype getShape();
    // }
}