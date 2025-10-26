// Assets/Scripts/Inventory/Slots/SimpleGridItemIndex.cs
using System.Collections.Generic;
using Inventory.Slots.Domain.Api;
using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.Slots.Domain
{
    /// Trzyma spójne mapy: cell->item oraz item->origin. Nie modyfikuje InventoryGrid.
    public class SimpleGridItemIndex : IGridItemIndex
    {
        private readonly Dictionary<Vector2Int, ItemData> _cellToItem = new();
        private readonly Dictionary<ItemData, Vector2Int> _itemToOrigin = new();

        public bool TryGetItemAtCell(Vector2Int cell, out ItemData item, out Vector2Int origin)
        {
            if (_cellToItem.TryGetValue(cell, out item) && _itemToOrigin.TryGetValue(item, out origin))
                return true;

            item = null;
            origin = default;
            return false;
        }

        public IEnumerable<Vector2Int> GetOccupiedCells(ItemData item, Vector2Int origin)
        {
            // Bazuje na Shape.Cells ItemData (offsety względem origin).
            foreach (var off in item.Shape.Cells)
                yield return origin + off;
        }

        public void Register(ItemData item, Vector2Int origin)
        {
            _itemToOrigin[item] = origin;
            foreach (var cell in GetOccupiedCells(item, origin))
                _cellToItem[cell] = item;
        }

        public void Unregister(ItemData item)
        {
            if (!_itemToOrigin.TryGetValue(item, out var origin))
                return;

            foreach (var cell in GetOccupiedCells(item, origin))
                _cellToItem.Remove(cell);

            _itemToOrigin.Remove(item);
        }

        public bool Contains(ItemData item) => _itemToOrigin.ContainsKey(item);
    }
}