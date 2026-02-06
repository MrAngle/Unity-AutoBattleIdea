using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IGridInspector {
        bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item);
    }
}