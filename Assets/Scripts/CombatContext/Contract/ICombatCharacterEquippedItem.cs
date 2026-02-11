using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterEquippedItem {
        public long getId();
        public Vector2Int getOrigin();
        public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        public ShapeArchetype getShape();
        public IActionDescription prepareItemActionDescription();
    }
}