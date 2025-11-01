using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Position {
    public interface IInventoryPosition {
        public IReadOnlyCollection<Vector2Int> GetOccupiedCells();
    }
}