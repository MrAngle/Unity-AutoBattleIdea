using MageFactory.Shared.Event;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Api.Event {
    public readonly struct CombatCharacterCreatedDtoEvent : IDomainEvent {
    }

    public interface ICombatCharacterCreatedEventListener
        : IDomainEventListener<CombatCharacterCreatedDtoEvent> {
    }

    public readonly struct CombatContextCreatedDtoEvent : IDomainEvent {
        public readonly ICombatContext combatContext;

        public CombatContextCreatedDtoEvent(ICombatContext combatContext) {
            this.combatContext = combatContext;
        }
    }

    public interface ICombatContextEventListener
        : IDomainEventListener<CombatContextCreatedDtoEvent> {
    }

    public readonly struct FlowGuardCreatedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly Id<GuardId> guardId;
        public readonly long guardPower;
        public readonly ItemFlowProcessingSlot sourceProcessingSlot;
        public readonly bool replacedGuard;
        public readonly Id<GuardId> replacedGuardId;
        public readonly long replacedGuardPower;

        public FlowGuardCreatedDtoEvent(
            Id<CharacterId> characterId,
            Id<GuardId> guardId,
            long guardPower,
            ItemFlowProcessingSlot sourceProcessingSlot)
            : this(characterId, guardId, guardPower, sourceProcessingSlot, false, default, 0) {
        }

        public FlowGuardCreatedDtoEvent(
            Id<CharacterId> characterId,
            Id<GuardId> guardId,
            long guardPower,
            ItemFlowProcessingSlot sourceProcessingSlot,
            bool replacedGuard,
            Id<GuardId> replacedGuardId,
            long replacedGuardPower) {
            this.characterId = characterId;
            this.guardId = guardId;
            this.guardPower = guardPower;
            this.sourceProcessingSlot = sourceProcessingSlot;
            this.replacedGuard = replacedGuard;
            this.replacedGuardId = replacedGuardId;
            this.replacedGuardPower = replacedGuardPower;
        }

        public bool hasSourceProcessingSlot() {
            return sourceProcessingSlot != null;
        }
    }

    public interface IFlowGuardCreatedEventListener
        : IDomainEventListener<FlowGuardCreatedDtoEvent> {
    }

    public readonly struct FlowStabilityCreatedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long stabilityPower;
        public readonly long stabilityBefore;
        public readonly long stabilityAfter;
        public readonly long baselineStability;
        public readonly ItemFlowProcessingSlot sourceProcessingSlot;

        public FlowStabilityCreatedDtoEvent(
            Id<CharacterId> characterId,
            long stabilityPower,
            long stabilityBefore,
            long stabilityAfter,
            long baselineStability,
            ItemFlowProcessingSlot sourceProcessingSlot) {
            this.characterId = characterId;
            this.stabilityPower = stabilityPower;
            this.stabilityBefore = stabilityBefore;
            this.stabilityAfter = stabilityAfter;
            this.baselineStability = baselineStability;
            this.sourceProcessingSlot = sourceProcessingSlot;
        }

        public bool hasSourceProcessingSlot() {
            return sourceProcessingSlot != null;
        }
    }

    public interface IFlowStabilityCreatedEventListener
        : IDomainEventListener<FlowStabilityCreatedDtoEvent> {
    }

    public readonly struct FlowInputStartedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly Id<ItemId> inputItemId;

        public FlowInputStartedDtoEvent(
            Id<CharacterId> characterId,
            Id<ItemId> inputItemId) {
            this.characterId = characterId;
            this.inputItemId = inputItemId;
        }
    }

    public interface IFlowInputStartedEventListener
        : IDomainEventListener<FlowInputStartedDtoEvent> {
    }

    public readonly struct FlowOutputReachedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long attackPower;
        public readonly long guardPower;
        public readonly long stabilityPower;
        public readonly ItemFlowProcessingSlot outputProcessingSlot;

        public FlowOutputReachedDtoEvent(
            Id<CharacterId> characterId,
            long attackPower,
            long guardPower,
            long stabilityPower,
            ItemFlowProcessingSlot outputProcessingSlot) {
            this.characterId = characterId;
            this.attackPower = attackPower;
            this.guardPower = guardPower;
            this.stabilityPower = stabilityPower;
            this.outputProcessingSlot = outputProcessingSlot;
        }

        public bool hasOutputProcessingSlot() {
            return outputProcessingSlot != null;
        }
    }

    public interface IFlowOutputReachedEventListener
        : IDomainEventListener<FlowOutputReachedDtoEvent> {
    }

    public readonly struct FlowNoOutputDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long attackPower;
        public readonly long guardPower;
        public readonly long stabilityPower;
        public readonly ItemFlowProcessingSlot finalProcessingSlot;
        public readonly bool wasCommittedByLegacyRule;

        public FlowNoOutputDtoEvent(
            Id<CharacterId> characterId,
            long attackPower,
            long guardPower,
            long stabilityPower,
            ItemFlowProcessingSlot finalProcessingSlot,
            bool wasCommittedByLegacyRule) {
            this.characterId = characterId;
            this.attackPower = attackPower;
            this.guardPower = guardPower;
            this.stabilityPower = stabilityPower;
            this.finalProcessingSlot = finalProcessingSlot;
            this.wasCommittedByLegacyRule = wasCommittedByLegacyRule;
        }

        public bool hasFinalProcessingSlot() {
            return finalProcessingSlot != null;
        }
    }

    public interface IFlowNoOutputEventListener
        : IDomainEventListener<FlowNoOutputDtoEvent> {
    }

    public readonly struct FlowAttackCreatedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> sourceCharacterId;
        public readonly Id<CharacterId> targetCharacterId;
        public readonly long attackPower;
        public readonly ItemFlowProcessingSlot sourceProcessingSlot;

        public FlowAttackCreatedDtoEvent(
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            long attackPower,
            ItemFlowProcessingSlot sourceProcessingSlot) {
            this.sourceCharacterId = sourceCharacterId;
            this.targetCharacterId = targetCharacterId;
            this.attackPower = attackPower;
            this.sourceProcessingSlot = sourceProcessingSlot;
        }

        public bool hasSourceProcessingSlot() {
            return sourceProcessingSlot != null;
        }
    }

    public interface IFlowAttackCreatedEventListener
        : IDomainEventListener<FlowAttackCreatedDtoEvent> {
    }

    public enum DamagePacketLayer {
        Travel,
        IncomingHit,
        Stability,
        Guard,
        Hp
    }

    public readonly struct DamagePacketLayerProcessedDtoEvent : IDomainEvent {
        public readonly long packetId;
        public readonly Id<CharacterId> sourceCharacterId;
        public readonly Id<CharacterId> targetCharacterId;
        public readonly DamagePacketLayer layer;
        public readonly long incomingDamage;
        public readonly long outgoingDamage;
        public readonly int ticksToNextLayer;
        public readonly bool completesPacket;

        public DamagePacketLayerProcessedDtoEvent(
            long packetId,
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            DamagePacketLayer layer,
            long incomingDamage,
            long outgoingDamage,
            int ticksToNextLayer)
            : this(
                packetId,
                sourceCharacterId,
                targetCharacterId,
                layer,
                incomingDamage,
                outgoingDamage,
                ticksToNextLayer,
                false) {
        }

        public DamagePacketLayerProcessedDtoEvent(
            long packetId,
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            DamagePacketLayer layer,
            long incomingDamage,
            long outgoingDamage,
            int ticksToNextLayer,
            bool completesPacket) {
            this.packetId = packetId;
            this.sourceCharacterId = sourceCharacterId;
            this.targetCharacterId = targetCharacterId;
            this.layer = layer;
            this.incomingDamage = incomingDamage;
            this.outgoingDamage = outgoingDamage;
            this.ticksToNextLayer = ticksToNextLayer;
            this.completesPacket = completesPacket;
        }
    }

    public interface IDamagePacketLayerProcessedEventListener
        : IDomainEventListener<DamagePacketLayerProcessedDtoEvent> {
    }
}