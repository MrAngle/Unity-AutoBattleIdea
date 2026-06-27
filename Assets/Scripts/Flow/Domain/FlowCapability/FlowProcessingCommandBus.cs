using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingCommandBus {
        private readonly FlowContext flowContext;

        public FlowProcessingCommandBus(FlowContext flowContext) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
        }

        internal void consumeFlow(ItemFlowProcessingSlot finalProcessingSlot, bool reachedOutputPort) {
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();

            ConsumeFlowCommand offensiveFlowCommand =
                new(flowContext.getFlowKind(),
                    flowContext.getFlowOwner(),
                    flowContext.getAttackPower(),
                    flowContext.getGuardPower(),
                    flowContext.getStabilityPower(),
                    flowContext.getSourceCharacterId(),
                    finalProcessingSlot,
                    reachedOutputPort);
            flowConsumer.consumeFlow(offensiveFlowCommand);
        }

        internal void discardFlow(ItemFlowProcessingSlot finalProcessingSlot) {
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();

            DiscardFlowCommand discardFlowCommand =
                new(flowContext.getFlowKind(),
                    flowContext.getFlowOwner(),
                    flowContext.getAttackPower(),
                    flowContext.getGuardPower(),
                    flowContext.getStabilityPower(),
                    flowContext.getSourceCharacterId(),
                    finalProcessingSlot);
            flowConsumer.discardFlow(discardFlowCommand);
        }

        internal ItemFlowProcessingSlot startProcessingFlowItem(IFlowItem item) {
            return flowContext.getFlowOwner().startProcessingFlowItem(item);
        }

        internal void finishProcessingFlowItem(ItemFlowProcessingSlot processingSlot) {
            flowContext.getFlowOwner().finishProcessingFlowItem(processingSlot);
        }
    }
}