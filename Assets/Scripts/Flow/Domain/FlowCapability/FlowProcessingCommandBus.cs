using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingCommandBus {
        private readonly FlowContext flowContext;

        public FlowProcessingCommandBus(FlowContext flowContext) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
        }

        internal void consumeFlow() {
            // it may return some results etc
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();
            FlowPayload flowPayload = flowContext.getFlowPayload();

            ProcessFlowCommand flowCommand =
                new(flowContext.getFlowOwner(), flowPayload.getDamageToDeal());
            flowConsumer.consumeFlow(flowCommand);
        }
    }
}