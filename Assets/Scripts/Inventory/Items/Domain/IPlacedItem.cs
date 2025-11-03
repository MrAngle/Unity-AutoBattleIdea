using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using UnityEngine;

namespace Inventory.Items.Domain {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        public void Process(FlowAggregate flowAggregate);

        public Task ProcessAsync(FlowAggregate flowAggregate, CancellationToken ct = default);

        public ShapeArchetype GetShape();
    }

    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}