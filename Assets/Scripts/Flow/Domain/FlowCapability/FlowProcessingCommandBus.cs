using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingCommandBus {
        private readonly FlowContext flowContext;

        public FlowProcessingCommandBus(FlowContext flowContext) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
        }

        internal void consumeFlow() {
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();

            ConsumeFlowCommand offensiveFlowCommand =
                new(flowContext.getFlowKind(),
                    flowContext.getFlowOwner(),
                    DamageToDeal.fromPowerAmount(flowContext
                        .getAttackPower()));
            flowConsumer.consumeFlow(offensiveFlowCommand);
        }

        internal ItemFlowProcessingSlot startProcessingFlowItem(IFlowItem item) {
            return flowContext.getFlowOwner().startProcessingFlowItem(item);
        }

        internal void finishProcessingFlowItem(ItemFlowProcessingSlot processingSlot) {
            flowContext.getFlowOwner().finishProcessingFlowItem(processingSlot);
        }
    }
}