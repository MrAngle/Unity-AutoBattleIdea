using Contracts.Actionexe;
using Contracts.Inventory;
using UnityEngine;

namespace Contracts.Items {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        // public void Process(FlowAggregate flowAggregate);

        public IActionSpecification GetAction();

        // public Task ProcessAsync(FlowAggregate flowAggregate, CancellationToken ct = default);

        public ShapeArchetype GetShape();
    }

    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}