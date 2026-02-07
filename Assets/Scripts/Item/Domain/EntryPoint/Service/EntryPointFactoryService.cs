using MageFactory.Flow.Api;
using MageFactory.Item.Api;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using Zenject;

namespace MageFactory.Item.Domain.EntryPoint.Service {
    public class EntryPointFactoryService : IEntryPointFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        public EntryPointFactoryService(IFlowFactory flowFactory) {
            _flowFactory = flowFactory;
        }

        // @Override
        public IPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector) {
            var placedEntryPoint = PlacedEntryPoint.Create(archetype, position, gridInspector, _flowFactory);

            return placedEntryPoint;
        }

        // @Override
        public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
            return new TickEntryPoint(kind, shapeArchetype, this);
        }
    }
}