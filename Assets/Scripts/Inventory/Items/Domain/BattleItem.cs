using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
using UnityEngine;
using Random = System.Random;

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

        public long GetId() {
            return _id;
        }

        public void Process(FlowAggregate flowAggregate) {
            // Na ten moment na tym poziomie, ale byc moze do wyciagniecia na aggregate
            Debug.Log("Processing battle item");

            flowAggregate.AddPower(5);
            
            // throw new NotImplementedException();
        }
    }
}