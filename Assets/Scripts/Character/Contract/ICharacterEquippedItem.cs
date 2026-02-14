using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedItem : ICombatCharacterEquippedItem {
        public long getId();
        public Vector2Int getOrigin();
        public IReadOnlyCollection<Vector2Int> getOccupiedCells();

        public ShapeArchetype getShape();
        // public IActionDescription prepareItemActionDescription();
    }
}