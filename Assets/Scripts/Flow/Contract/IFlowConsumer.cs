using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Contract {
    public readonly struct ConsumeFlowCommand {
        public readonly FlowKind flowKind;
        public readonly IFlowOwner flowOwner;
        public readonly PowerAmount attackPower;
        public readonly GuardPower guardPower;
        public readonly Id<CharacterId> sourceCharacterId;
        public readonly ItemFlowProcessingSlot finalProcessingSlot;
        public readonly bool reachedOutputPort;

        public ConsumeFlowCommand(FlowKind flowKind,
                                  IFlowOwner flowOwner,
                                  PowerAmount attackPower,
                                  GuardPower guardPower,
                                  Id<CharacterId> sourceCharacterId)
            : this(flowKind, flowOwner, attackPower, guardPower, sourceCharacterId, null, false) {
        }

        public ConsumeFlowCommand(FlowKind flowKind,
                                  IFlowOwner flowOwner,
                                  PowerAmount attackPower,
                                  GuardPower guardPower,
                                  Id<CharacterId> sourceCharacterId,
                                  ItemFlowProcessingSlot finalProcessingSlot)
            : this(flowKind, flowOwner, attackPower, guardPower, sourceCharacterId, finalProcessingSlot, false) {
        }

        public ConsumeFlowCommand(FlowKind flowKind,
                                  IFlowOwner flowOwner,
                                  PowerAmount attackPower,
                                  GuardPower guardPower,
                                  Id<CharacterId> sourceCharacterId,
                                  ItemFlowProcessingSlot finalProcessingSlot,
                                  bool reachedOutputPort) {
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.attackPower = NullGuard.NotNullOrThrow(attackPower);
            this.guardPower = NullGuard.NotNullOrThrow(guardPower);
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.sourceCharacterId = sourceCharacterId;
            this.finalProcessingSlot = finalProcessingSlot;
            this.reachedOutputPort = reachedOutputPort;
        }

        public bool hasSourceCharacterId() {
            return sourceCharacterId.Value > 0;
        }

        public bool hasAttackPower() {
            return attackPower.getPower() > 0;
        }

        public bool hasGuardPower() {
            return guardPower.getPower() > 0;
        }

        public bool hasFinalProcessingSlot() {
            return finalProcessingSlot != null;
        }
    }

    public readonly struct DiscardFlowCommand {
        public readonly FlowKind flowKind;
        public readonly IFlowOwner flowOwner;
        public readonly PowerAmount attackPower;
        public readonly GuardPower guardPower;
        public readonly Id<CharacterId> sourceCharacterId;
        public readonly ItemFlowProcessingSlot finalProcessingSlot;

        public DiscardFlowCommand(FlowKind flowKind,
                                  IFlowOwner flowOwner,
                                  PowerAmount attackPower,
                                  GuardPower guardPower,
                                  Id<CharacterId> sourceCharacterId,
                                  ItemFlowProcessingSlot finalProcessingSlot) {
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.attackPower = NullGuard.NotNullOrThrow(attackPower);
            this.guardPower = NullGuard.NotNullOrThrow(guardPower);
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.sourceCharacterId = sourceCharacterId;
            this.finalProcessingSlot = finalProcessingSlot;
        }

        public bool hasFinalProcessingSlot() {
            return finalProcessingSlot != null;
        }
    }

    public interface IFlowConsumer {
        void consumeFlow(ConsumeFlowCommand consumeFlowCommand); // TODO flow response etc.
        void discardFlow(DiscardFlowCommand discardFlowCommand);
    }
}