using System.Threading;
using MageFactory.Item.Api;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    public abstract class EntryPointArchetype : IEntryPointArchetype {
        private readonly IEntryPointFactory _entryPointFactory; // separate in future
        private readonly FlowKind _kind;
        private readonly ShapeArchetype _shapeArchetype;

        private readonly float _turnInterval;

        private bool _battleRunning;
        private CancellationTokenSource _cts;

        protected EntryPointArchetype(FlowKind kind, ShapeArchetype shapeArchetype,
            IEntryPointFactory entryPointFactory) {
            _entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);

            _kind = kind;
            _shapeArchetype = NullGuard.NotNullOrThrow(shapeArchetype);

            _turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        public IPlacedItem toPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return _entryPointFactory.createPlacedEntryPoint(this, origin, gridInspector);
        }

        public ShapeArchetype getShape() {
            return _shapeArchetype;
        }

        public FlowKind getFlowKind() {
            return _kind;
        }

        public float getTurnInterval() {
            return _turnInterval;
        }

        public override string ToString() {
            return $"({_kind})";
        }
    }
}