using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
using TimeSystem;
using UnityEngine;

namespace Inventory.Items.Domain {

    internal class BattleItem : IPlacedItem {
        // private readonly ShapeArchetype _shapeArchetype;
        private readonly long _id;
        private readonly ItemArchetype _itemArchetype;
        private readonly InventoryPosition _inventoryPosition;
        
        internal BattleItem(ItemArchetype itemArchetype, Vector2Int origin) {
            _id = IdGenerator.Next();
            _itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            _inventoryPosition = InventoryPosition.Create(origin, _itemArchetype.GetShape().Shape);
            // Register(Data, Origin);
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _inventoryPosition.GetOccupiedCells();
        }

        public Vector2Int GetOrigin() {
            return _inventoryPosition.GetOrigin();
        }

        public long GetId() {
            return _id;
        }

        async public void Process(FlowAggregate flowAggregate) {
            await TimeModule.ContinueIn(5f); 
            flowAggregate.AddPower(5);
        }
        
        public async Task ProcessAsync(FlowAggregate flowAggregate, CancellationToken ct = default) {
            await TimeModule.ContinueIn(0.1f, ct);
            flowAggregate.AddPower(5);
        }

        public ShapeArchetype GetShape() {
            return _itemArchetype.GetShape();
        }

    }
}