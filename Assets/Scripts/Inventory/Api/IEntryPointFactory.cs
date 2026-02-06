using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IEntryPointFactory {
        IEntryPointArchetype CreateArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);

        IPlacedEntryPoint CreatePlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector);
    }
}