using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Contract {
    public readonly struct ConsumeFlowCommand {
        public readonly FlowKind flowKind;
        public readonly IFlowOwner flowOwner;
        public readonly PowerAmount attackPower;
        public readonly Id<CharacterId> sourceCharacterId;

        public ConsumeFlowCommand(FlowKind flowKind,
                                  IFlowOwner flowOwner,
                                  PowerAmount attackPower,
                                  Id<CharacterId> sourceCharacterId) {
            this.flowOwner = flowOwner;
            this.attackPower = attackPower;
            this.flowKind = flowKind;
            this.sourceCharacterId = sourceCharacterId;
        }

        public bool hasSourceCharacterId() {
            return sourceCharacterId.Value > 0;
        }
    }

    public interface IFlowConsumer {
        void consumeFlow(ConsumeFlowCommand consumeFlowCommand); // TODO flow response etc.
    }
}