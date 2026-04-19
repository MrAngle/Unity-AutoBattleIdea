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
            // it may return some results etc
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();

            ConsumeFlowCommand offensiveFlowCommand =
                new(flowContext.getFlowKind(),
                    flowContext.getFlowOwner(),
                    DamageToDeal.fromPowerAmount(flowContext
                        .getAttackPower())); // flow should decide what to do with damage
            flowConsumer.consumeFlow(offensiveFlowCommand);
        }
    }
}