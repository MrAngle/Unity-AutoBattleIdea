using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.Item.Controller.Api {
    public interface IInventoryPosition {
        public IReadOnlyCollection<Vector2Int> getOccupiedCells();
        public Vector2Int getOrigin();
    }
}