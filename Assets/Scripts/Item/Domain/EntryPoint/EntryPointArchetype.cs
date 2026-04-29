using System.Threading;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;

namespace MageFactory.Item.Domain.EntryPoint {
    internal abstract class EntryPointArchetype : IEntryPointArchetype {
        private readonly IEntryPointFactory entryPointFactory; // separate in future
        private readonly IEntryPointDefinition entryPointDefinition;

        private bool isBattleRunning;
        private CancellationTokenSource cancellationTokenSource;

        protected EntryPointArchetype(IEntryPointDefinition entryPointDefinition,
                                      IEntryPointFactory entryPointFactory) {
            this.entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);
            this.entryPointDefinition = NullGuard.NotNullOrThrow(entryPointDefinition);
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

        public FlowKind getFlowKind() {
            return entryPointDefinition.getFlowKind();
        }

        public CombatTicks getTriggerInterval() {
            return entryPointDefinition.getTriggerIntervalTicks();
        }

        public IEntryPointDefinition getEntryPointDefinition() {
            return entryPointDefinition;
        }

        public IEntryPointDefinition getItemDefinition() {
            return entryPointDefinition;
        }

        public override string ToString() {
            return $"({getFlowKind()})";
        }
    }
}