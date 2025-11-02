using System.Collections.Generic;
using Combat.Flow.Domain;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using UnityEngine;

namespace Inventory.Items.Domain {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        public void Process(FlowAggregate flowAggregate);
        
        public ShapeArchetype GetShape();
    }
    
    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}