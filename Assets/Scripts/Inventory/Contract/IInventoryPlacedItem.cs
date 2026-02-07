using System.Collections.Generic;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Character.Contract;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedItem : ICharacterEquippedItem {
        public long getId();
        public IActionDescription prepareItemActionDescription();
        public ShapeArchetype getShape();

        public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        public Vector2Int getOrigin();
    }
}