using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPosition {
        public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        public Vector2Int getOrigin();
    }
}