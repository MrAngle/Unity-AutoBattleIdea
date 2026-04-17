using MageFactory.Shared.Id;

namespace MageFactory.CombatEvents {
    internal class DamageIncomingCombatEvent : CombatEvent {
        private readonly Id<CharacterId> sourceCharacterId;
        private readonly DamageSourceType damageSourceType;
        private readonly int rawDamage;

        public DamageIncomingCombatEvent(
            Id<CharacterId> targetCharacterId,
            Id<CharacterId> sourceCharacterId,
            DamageSourceType damageSourceType,
            int rawDamage
        ) : base(targetCharacterId) {
            this.sourceCharacterId = sourceCharacterId;
            this.damageSourceType = damageSourceType;
            this.rawDamage = rawDamage;
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

        public int getRawDamage() {
            return rawDamage;
        }
    }
}