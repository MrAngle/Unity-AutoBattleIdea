using System.Threading;
using MageFactory.Item.Api;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal abstract class EntryPointArchetype : IEntryPointArchetype {
        private readonly IEntryPointFactory entryPointFactory; // separate in future
        private readonly FlowKind flowKind;
        private readonly ShapeArchetype shapeArchetype;
        private readonly float turnInterval;

        private bool isBattleRunning;
        private CancellationTokenSource cancellationTokenSource;

        protected EntryPointArchetype(FlowKind flowKind, ShapeArchetype shapeArchetype,
            IEntryPointFactory entryPointFactory) {
            this.entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);

            this.flowKind = flowKind;
            this.shapeArchetype = NullGuard.NotNullOrThrow(shapeArchetype);

            turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        public IPlacedItem toPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return entryPointFactory.createPlacedEntryPoint(this, origin, gridInspector);
        }

        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        public FlowKind getFlowKind() {
            return flowKind;
        }

        public float getTurnInterval() {
            return turnInterval;
        }

        public override string ToString() {
            return $"({flowKind})";
        }
    }
}