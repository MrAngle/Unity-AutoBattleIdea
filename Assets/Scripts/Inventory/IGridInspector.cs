using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory {
    public interface IGridInspector {
        bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item);
    }
}