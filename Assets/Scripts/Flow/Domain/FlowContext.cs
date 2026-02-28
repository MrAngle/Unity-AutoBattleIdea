using MageFactory.Flow.Contract;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly IFlowItem placedEntryPoint;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowOwner flowOwner;
        private readonly IFlowCapabilities flowCapabilities;
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

        public bool pushRightAdjacentItemRight(IFlowItem sourceItem) {
            if (flowCapabilities.query().tryGetRightAdjacentItem(sourceItem, out IFlowItem adjacentItem)) {
                if (flowCapabilities.command().tryMoveItemToRight(adjacentItem)) {
                    return true;
                }
            }

            return false;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}