using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IGridInspector {
        bool tryGetItemAtCell(Vector2Int cell, out IPlacedItem item);
    }
}