using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
using UnityEngine;
using Random = System.Random;

namespace Inventory.Items.Domain {

    internal class BattleItem : IPlacedItem {
        private readonly ItemData _data;
        private readonly long _id;
        private readonly InventoryPosition _inventoryPosition;

        
        internal BattleItem(ItemData data, Vector2Int origin) {
            _id = IdGenerator.Next();
            _data = data;
            _inventoryPosition = InventoryPosition.Create(origin, _data.Shape);
            // Register(Data, Origin);
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _inventoryPosition.GetOccupiedCells();
        }

        public long GetId() {
            return _id;
        }

        public void Process(FlowAggregate flowAggregate) {
            flowAggregate.AddPower(5);
        }
    }
}