using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatEvents {
    public sealed class DamageIncomingCombatEvent : CombatEvent {
        private readonly Id<CharacterId> sourceCharacterId;
        private readonly DamageSourceType damageSourceType;
        private readonly DamageToReceive rawDamageToReceive;

        public DamageIncomingCombatEvent(
            Id<CharacterId> targetCharacterId,
            Id<CharacterId> sourceCharacterId,
            DamageSourceType damageSourceType,
            DamageToDeal rawDamageToDeal
        ) : base(targetCharacterId) {
            this.sourceCharacterId = sourceCharacterId;
            this.damageSourceType = damageSourceType;
            this.rawDamageToReceive = NullGuard.NotNullOrThrow(DamageToReceive.fromDamageToDeal(rawDamageToDeal));
        }

        public override CombatEventType getType() {
            return CombatEventType.DAMAGE_INCOMING;
        }

        public Id<CharacterId> getSourceCharacterId() {
            return sourceCharacterId;
        }

        public DamageSourceType getDamageSourceType() {
            return damageSourceType;
        }

        public DamageToReceive getRawDamageToReceive() {
            return rawDamageToReceive;
        }
    }
}