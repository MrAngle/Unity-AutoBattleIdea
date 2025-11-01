using System.Collections.Generic;
using Combat.Flow.Domain;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using UnityEngine;

namespace Inventory.Items.Domain {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        public void Process(FlowAggregate flowAggregate);
        
        public static IPlacedItem CreateBattleItem(ItemData data, Vector2Int origin) {
            return new BattleItem(data, origin);
        }
    }
}