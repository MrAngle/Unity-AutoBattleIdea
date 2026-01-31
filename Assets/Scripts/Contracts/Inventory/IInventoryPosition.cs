using System.Collections.Generic;
using UnityEngine;

namespace Contracts.Inventory {
    public interface IInventoryPosition {
        public IReadOnlyCollection<Vector2Int> GetOccupiedCells();
        public Vector2Int GetOrigin();
    }
}