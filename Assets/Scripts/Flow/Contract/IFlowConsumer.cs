using MageFactory.Shared.Model;

namespace MageFactory.Flow.Contract {
    public readonly struct ProcessFlowCommand {
        public readonly IFlowOwner flowOwner;
        public readonly DamageToDeal damageToDeal;

        public ProcessFlowCommand(IFlowOwner flowOwner, DamageToDeal damageToDeal) {
            this.flowOwner = flowOwner;
            this.damageToDeal = damageToDeal;
        }
    }

    public interface IFlowConsumer {
        DamageToDeal consumeFlow(ProcessFlowCommand flowCommand); // TODO flow response etc.
    }
}