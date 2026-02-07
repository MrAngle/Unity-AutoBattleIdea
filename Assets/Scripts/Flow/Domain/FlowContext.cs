using MageFactory.Item.Controller.Api;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly IPlacedEntryPoint placedEntryPoint;
        private int stepIndex;

        internal FlowContext(IPlacedEntryPoint placedEntryPoint) {
            this.placedEntryPoint = placedEntryPoint;
        }

        internal IPlacedEntryPoint getPlacedEntryPoint() {
            return placedEntryPoint;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}