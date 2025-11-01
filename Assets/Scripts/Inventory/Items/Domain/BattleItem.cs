using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
using UnityEngine;

namespace Inventory.Items.Domain {
    // public interface IPlacedItem : IFlowNode {
    //     public ICollection<Vector2Int> GetOccupiedCells();
    //     
    //     public static IPlacedItem CreateBattleItem(ItemData data, Vector2Int origin) {
    //         return new PlacedItem(data, origin);
    //     }
    //     
    //     public static IPlacedItem CreateEntryPoint(Vector2Int origin) {
    //         return new PlacedItem(data, origin);
    //     }
    // }

    internal class BattleItem : IPlacedItem {
        private readonly ItemData _data;

        private readonly long _id;
        // private readonly Dictionary<Vector2Int, ItemData> _cellToItem = new();
        // private readonly Dictionary<ItemData, Vector2Int> _itemToOrigin = new();

        private readonly InventoryPosition _inventoryPosition;
        private readonly Vector2Int _origin;

        internal BattleItem(ItemData data, Vector2Int origin) {
            _id = IdGenerator.Next();
            _data = data;
            _origin = origin;
            _inventoryPosition = InventoryPosition.Create(_origin, _data.Shape);
            // Register(Data, Origin);
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _inventoryPosition.GetOccupiedCells();
        }

        // public ICollection<Vector2Int> GetOccupiedCells() {
        //     return _cellToItem.Keys.ToHashSet();
        //     // return Data.Shape.Cells.Select(offset => Origin + offset);
        // }

        // public bool TryGetItemAtCell(Vector2Int cell, out ItemData item, out Vector2Int origin)
        // {
        //     if (_cellToItem.TryGetValue(cell, out item) && _itemToOrigin.TryGetValue(item, out origin))
        //         return true;
        //
        //     item = null;
        //     origin = default;
        //     return false;
        // }

        // public IEnumerable<Vector2Int> GetOccupiedCells(ItemData item, Vector2Int origin)
        // {
        //     // Bazuje na Shape.Cells ItemData (offsety względem origin).
        //     foreach (var off in item.Shape.Cells)
        //         yield return origin + off;
        // }

        // public void Register(ItemData item, Vector2Int origin)
        // {
        //     _itemToOrigin[item] = origin;
        //     foreach (var cell in GetOccupiedCells(item, origin))
        //         _cellToItem[cell] = item;
        // }

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

        public long GetId() {
            return _id;
        }

        public void Process(FlowAggregate flowAggregate) {
            // Na ten moment na tym poziomie, ale byc moze do wyciagniecia na aggregate

            throw new NotImplementedException();
        }
    }
}