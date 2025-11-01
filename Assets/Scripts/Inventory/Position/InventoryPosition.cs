using System.Collections.Generic;
using System.Linq;
using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.Position {
    public class InventoryPosition : IInventoryPosition {
        private Vector2Int _origin;
        private ItemShape _itemShape;
        
        private HashSet<Vector2Int> _occupiedCells;

        public static InventoryPosition Create(Vector2Int origin, ItemShape itemItemShape) {
            return new InventoryPosition(origin, itemItemShape);
        }

        private InventoryPosition(Vector2Int origin, ItemShape itemItemShape) {
            _origin = origin;
            _itemShape = itemItemShape;
            _occupiedCells = CalculateOccupiedCellsByOrigin();
        }
        
        // public IReadOnlyCollection<Vector2Int> GetOccupiedCells()
        //     => _cellToItem.Keys.ToArray();
        
        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _occupiedCells;
            // return Data.Shape.Cells.Select(offset => Origin + offset);
        }
        
        private HashSet<Vector2Int> CalculateOccupiedCellsByOrigin() {
            return _itemShape.GetCellSetAt(_origin);
            // var set = new HashSet<Vector2Int>();
            // foreach (var off in _itemShape.Cells)
            //     set.Add(_origin + off);
            // return set;
        }
        
        // public IEnumerable<Vector2Int> GetOccupiedCells(ItemData item, Vector2Int origin)
        // {
        //     // Bazuje na Shape.Cells ItemData (offsety względem origin).
        //     foreach (var off in item.Shape.Cells)
        //         yield return origin + off;
        // }
        
        // public void Register(ItemData item, Vector2Int origin)
        // {
        //     _origin = origin;
        //     foreach (var cell in GetOccupiedCells())
        //         _cellToItem[cell] = item;
        // }
        //
        // public void Unregister(ItemData item)
        // {
        //     foreach (var cell in GetOccupiedCells())
        //         _cellToItem.Remove(cell);
        // }
    }
}


// private readonly Dictionary<Vector2Int, ItemData> _cellToItem = new();
// private readonly Dictionary<ItemData, Vector2Int> _itemToOrigin = new();
// public bool TryGetItemAtCell(Vector2Int cell, out ItemData item, out Vector2Int origin)
// {
//     if (_cellToItem.TryGetValue(cell, out item) && _itemToOrigin.TryGetValue(item, out origin))
//         return true;
//
//     item = null;
//     origin = default;
//     return false;
// }
//
// public IEnumerable<Vector2Int> GetOccupiedCells(ItemData item, Vector2Int origin)
// {
//     // Bazuje na Shape.Cells ItemData (offsety względem origin).
//     foreach (var off in item.Shape.Cells)
//         yield return origin + off;
// }
//
// public void Register(ItemData item, Vector2Int origin)
// {
//     _itemToOrigin[item] = origin;
//     foreach (var cell in GetOccupiedCells(item, origin))
//         _cellToItem[cell] = item;
// }
//
// public void Unregister(ItemData item)
// {
//     if (!_itemToOrigin.TryGetValue(item, out var origin))
//         return;
//
//     foreach (var cell in GetOccupiedCells(item, origin))
//         _cellToItem.Remove(cell);
//
//     _itemToOrigin.Remove(item);
// }