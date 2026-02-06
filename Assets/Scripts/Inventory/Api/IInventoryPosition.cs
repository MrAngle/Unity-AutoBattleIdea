using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IInventoryPosition {
        public IReadOnlyCollection<Vector2Int> GetOccupiedCells();
        public Vector2Int GetOrigin();
    }
}