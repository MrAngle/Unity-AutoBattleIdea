using Contracts.Items;
using UnityEngine;

namespace Contracts.Inventory {
    public interface IGridInspector {
        bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item);
    }
}