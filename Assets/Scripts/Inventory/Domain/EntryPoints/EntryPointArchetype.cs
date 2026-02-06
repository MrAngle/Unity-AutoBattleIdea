using System.Threading;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
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

        public IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return _entryPointFactory.CreatePlacedEntryPoint(this, origin, gridInspector);
        }

        public ShapeArchetype GetShape() {
            return _shapeArchetype;
        }

        public FlowKind GetFlowKind() {
            return _kind;
        }

        public float GetTurnInterval() {
            return _turnInterval;
        }

        public override string ToString() {
            return $"({_kind})";
        }
    }
}