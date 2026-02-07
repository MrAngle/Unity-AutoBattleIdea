using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Api {
    public interface IEntryPointFactory {
        IEntryPointArchetype CreateArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);

        IPlacedEntryPoint CreatePlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector);
    }
}