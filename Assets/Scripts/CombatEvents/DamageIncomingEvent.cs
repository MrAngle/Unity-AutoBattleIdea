using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatEvents {
    public sealed class IncomingAttackDamageCombatEvent : CombatEvent {
        private readonly Id<CharacterId> sourceCharacterId;
        private readonly DamageToReceive rawDamageToReceive;

        public IncomingAttackDamageCombatEvent(
            Id<CharacterId> targetCharacterId,
            Id<CharacterId> sourceCharacterId,
            DamageToDeal rawDamageToDeal
        ) : base(targetCharacterId) {
            this.sourceCharacterId = NullGuard.ValidIdOrThrow(sourceCharacterId);
            rawDamageToReceive = NullGuard.NotNullOrThrow(DamageToReceive.fromDamageToDeal(rawDamageToDeal));
        }

        public override CombatEventType getType() {
            return CombatEventType.INCOMING_ATTACK_DAMAGE;
        }

        public Id<CharacterId> getSourceCharacterId() {
            return sourceCharacterId;
        }

        public DamageRole getDamageRole() {
            return DamageRole.ATTACK;
        }

        public DamageToReceive getRawDamageToReceive() {
            return rawDamageToReceive;
        }
    }
}