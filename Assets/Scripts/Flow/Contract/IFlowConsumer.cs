using MageFactory.Shared.Model;

namespace MageFactory.Flow.Contract {
    public readonly struct ConsumeFlowCommand {
        public readonly FlowKind flowKind;
        public readonly IFlowOwner flowOwner;
        public readonly DamageToDeal damageToDeal;

        public ConsumeFlowCommand(FlowKind flowKind, IFlowOwner flowOwner, DamageToDeal damageToDeal) {
            this.flowOwner = flowOwner;
            this.damageToDeal = damageToDeal;
            this.flowKind = flowKind;
        }
    }

    public interface IFlowConsumer {
        DamageToDeal consumeFlow(ConsumeFlowCommand consumeFlowCommand); // TODO flow response etc.
    }
}