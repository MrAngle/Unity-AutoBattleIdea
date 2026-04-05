using System.Threading;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal abstract class EntryPointArchetype : IEntryPointArchetype {
        private readonly IEntryPointFactory entryPointFactory; // separate in future
        private readonly IEntryPointDefinition entryPointDefinition;
        private readonly float turnInterval;

        private bool isBattleRunning;
        private CancellationTokenSource cancellationTokenSource;

        protected EntryPointArchetype(IEntryPointDefinition entryPointDefinition,
                                      IEntryPointFactory entryPointFactory) {
            this.entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);
            this.entryPointDefinition = NullGuard.NotNullOrThrow(entryPointDefinition);

            turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        public IInventoryPlacedItem toPlacedItem(IInventoryPosition inventoryPosition) {
            return entryPointFactory.createPlacedEntryPoint(this, inventoryPosition);
        }

        IItemDefinition IInventoryPlaceableItem.getItemDefinition() {
            return getItemDefinition();
        }

        public ShapeArchetype getShape() {
            return entryPointDefinition.getShape();
        }

        // public ShapeArchetype getShape() {
        //     return shapeArchetype;
        // }

        public FlowKind getFlowKind() {
            return entryPointDefinition.getFlowKind();
        }

        public IEntryPointDefinition getEntryPointDefinition() {
            return entryPointDefinition;
        }

        public IEntryPointDefinition getItemDefinition() {
            return entryPointDefinition;
        }

        public float getTurnInterval() {
            return turnInterval;
        }

        public override string ToString() {
            return $"({getFlowKind()})";
        }
    }
}