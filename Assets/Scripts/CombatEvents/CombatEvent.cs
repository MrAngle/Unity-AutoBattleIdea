using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatEvents {
    public abstract class CombatEvent {
        private readonly Id<CombatEventId> combatEventId;
        private readonly Id<CharacterId> targetCharacterId;

        protected CombatEvent(Id<CharacterId> targetCharacterId) {
            combatEventId = new Id<CombatEventId>(IdGenerator.Next());
            this.targetCharacterId = targetCharacterId;
        }

        public Id<CombatEventId> getId() {
            return combatEventId;
        }

        public Id<CharacterId> getTargetCharacterId() {
            return targetCharacterId;
        }

        public abstract CombatEventType getType();
    }

    public enum CombatEventType {
        DAMAGE_INCOMING,
        DAMAGE_RESOLVED,
        HEALTH_LOST
    }

    internal enum DamageSourceType {
        EnemyAttack,
        SelfDamage,
        Sacrifice
    }
}