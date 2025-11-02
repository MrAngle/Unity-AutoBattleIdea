using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using Shared.Utility;
using UnityEngine;

namespace Inventory.Items.Domain {
    public class ItemArchetype : IPlaceableItem {
        private readonly ShapeArchetype _shapeArchetype;
        
        public ItemArchetype(ShapeArchetype shapeArchetype) {
            _shapeArchetype = shapeArchetype;
        }

        public void Process(FlowAggregate flowAggregate) {
            flowAggregate.AddPower(5);
        }

  
        public IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return new BattleItem(this, origin);
        }

        public ShapeArchetype GetShape() {
            return _shapeArchetype;
        }
    }
}