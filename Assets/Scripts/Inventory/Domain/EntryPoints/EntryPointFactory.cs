using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using UnityEngine;
using Zenject;

namespace Inventory.EntryPoints {
    public interface IEntryPointFactory {
        IPlacedEntryPoint CreatePlacedEntryPoint(EntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector);

        EntryPointArchetype CreateArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);
    }

    public class EntryPointFactory : IEntryPointFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        public EntryPointFactory(IFlowFactory flowFactory) {
            _flowFactory = flowFactory;
        }

        public IPlacedEntryPoint CreatePlacedEntryPoint(EntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector) {
            var placedEntryPoint = PlacedEntryPoint.Create(archetype, position, gridInspector, _flowFactory);

            return placedEntryPoint;
        }

        public EntryPointArchetype CreateArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
            return new TickEntryPoint(kind, shapeArchetype, this);
        }
    }
}