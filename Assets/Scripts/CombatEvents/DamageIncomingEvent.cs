using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatEvents {
    public sealed class DamageIncomingCombatEvent : CombatEvent {
        private readonly Id<CharacterId> sourceCharacterId;
        private readonly DamageRole damageRole;
        private readonly DamageToReceive rawDamageToReceive;

        public DamageIncomingCombatEvent(
            Id<CharacterId> targetCharacterId,
            Id<CharacterId> sourceCharacterId,
            DamageRole damageRole,
            DamageToDeal rawDamageToDeal
        ) : base(targetCharacterId) {
            this.sourceCharacterId = sourceCharacterId;
            this.damageRole = damageRole;
            this.rawDamageToReceive = NullGuard.NotNullOrThrow(DamageToReceive.fromDamageToDeal(rawDamageToDeal));
        }

        public override CombatEventType getType() {
            return CombatEventType.DAMAGE_INCOMING;
        }

        public Id<CharacterId> getSourceCharacterId() {
            return sourceCharacterId;
        }

        public DamageRole getDamageSourceType() {
            return damageRole;
        }

        public DamageToReceive getRawDamageToReceive() {
            return rawDamageToReceive;
        }
    }
}