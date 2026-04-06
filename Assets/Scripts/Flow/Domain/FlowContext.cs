using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly FlowPayload flowPayload;
        private readonly IFlowItem startEntryPoint;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowOwner flowOwner;
        private readonly IFlowRouter router;

        private int stepIndex;

        internal FlowContext(IFlowItem startEntryPoint, IFlowConsumer flowConsumer, IFlowOwner flowOwner,
                             IFlowRouter router) {
            this.startEntryPoint = NullGuard.NotNullOrThrow(startEntryPoint);
            this.flowConsumer = NullGuard.NotNullOrThrow(flowConsumer);
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.router = NullGuard.NotNullOrThrow(router);
            flowPayload = new FlowPayload();
            NullGuard.NotNullCheckOrThrow(this.startEntryPoint, this.flowConsumer, this.flowOwner, flowPayload,
                this.router);
        }

        internal IFlowConsumer getFlowConsumer() {
            return flowConsumer;
        }

        internal IFlowOwner getFlowOwner() {
            return flowOwner;
        }

        internal IFlowRouter getFlowRouter() {
            return router;
        }

        internal FlowPayload getFlowPayload() {
            return flowPayload;
        }
    }
}