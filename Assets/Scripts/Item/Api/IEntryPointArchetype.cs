using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Api {
    public interface IEntryPointArchetype : IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin);
        ShapeArchetype GetShape();
        float GetTurnInterval();
        FlowKind GetFlowKind();
    }
}