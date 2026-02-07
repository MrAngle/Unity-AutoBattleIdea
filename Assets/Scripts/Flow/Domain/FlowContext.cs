using MageFactory.Inventory.Contract;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly IInventoryPlacedEntryPoint placedEntryPoint;
        private int stepIndex;

        internal FlowContext(IInventoryPlacedEntryPoint placedEntryPoint) {
            this.placedEntryPoint = placedEntryPoint;
        }

        internal IInventoryPlacedEntryPoint getPlacedEntryPoint() {
            return placedEntryPoint;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}