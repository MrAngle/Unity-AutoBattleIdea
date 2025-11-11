using System.Threading;
using System.Threading.Tasks;
using Character;
using Combat.ActionExecutor;
using Combat.Flow.Domain.Aggregate;
using Inventory.Position;
using UnityEngine;

namespace Inventory.Items.Domain {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        // public void Process(FlowAggregate flowAggregate);

        public IActionSpecification GetAction();

        // public Task ProcessAsync(FlowAggregate flowAggregate, CancellationToken ct = default);

        public ShapeArchetype GetShape();
    }

    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IPlacedItemOwner placedItemOwner, IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}