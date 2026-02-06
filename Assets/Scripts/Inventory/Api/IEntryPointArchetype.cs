using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IEntryPointArchetype : IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin);
        ShapeArchetype GetShape();
        float GetTurnInterval();
        FlowKind GetFlowKind();
    }
}