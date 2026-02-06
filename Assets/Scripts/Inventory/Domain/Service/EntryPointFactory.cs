using MageFactory.Flow.Api;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Domain.Service {
    public class EntryPointFactory : IEntryPointFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        public EntryPointFactory(IFlowFactory flowFactory) {
            _flowFactory = flowFactory;
        }

        // @Override
        public IPlacedEntryPoint CreatePlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector) {
            var placedEntryPoint = PlacedEntryPoint.Create(archetype, position, gridInspector, _flowFactory);

            return placedEntryPoint;
        }

        // @Override
        public IEntryPointArchetype CreateArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
            return new TickEntryPoint(kind, shapeArchetype, this);
        }
    }
}