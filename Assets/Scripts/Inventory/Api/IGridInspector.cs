using UnityEngine;

namespace MageFactory.Item.Controller.Api {
    public interface IGridInspector {
        bool tryGetItemAtCell(Vector2Int cell, out IPlacedItem item);
    }
}