using MageFactory.Flow.Contract;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly IFlowItem placedEntryPoint;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowOwner flowOwner;
        private int stepIndex;

        internal FlowContext(IFlowItem placedEntryPoint, IFlowConsumer flowConsumer, IFlowOwner flowOwner) {
            this.placedEntryPoint = placedEntryPoint;
            this.flowConsumer = flowConsumer;
            this.flowOwner = flowOwner;
        }

        internal IFlowConsumer getFlowConsumer() {
            return flowConsumer;
        }

        internal IFlowOwner getFlowOwner() {
            return flowOwner;
        }

        internal IFlowItem getPlacedEntryPoint() {
            return placedEntryPoint;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}