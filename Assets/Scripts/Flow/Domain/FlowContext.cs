using MageFactory.Flow.Contract;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly IFlowItem placedEntryPoint;
        private int stepIndex;

        internal FlowContext(IFlowItem placedEntryPoint) {
            this.placedEntryPoint = placedEntryPoint;
        }

        internal IFlowItem getPlacedEntryPoint() {
            return placedEntryPoint;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}