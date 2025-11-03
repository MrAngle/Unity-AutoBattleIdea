using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
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

        public void Process(FlowAggregate flowAggregate) {
            flowAggregate.AddPower(5);
        }

        public ShapeArchetype GetShape() {
            return _itemArchetype.GetShape();
        }

    }
}