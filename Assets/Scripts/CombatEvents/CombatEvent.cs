using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

// 1 event = 1 charcter. Its very important to keep this in mind
namespace MageFactory.CombatEvents {
    public abstract class CombatEvent {
        private readonly Id<CombatEventId> combatEventId;
        private readonly Id<CharacterId> targetCharacterId; // it may be generics, but for now it's ok

        protected CombatEvent(Id<CharacterId> targetCharacterId) {
            combatEventId = new Id<CombatEventId>(IdGenerator.Next());
            this.targetCharacterId = targetCharacterId;
        }

        public Id<CombatEventId> getEventId() {
            return combatEventId;
        }

        public Id<CharacterId> getTargetCharacterId() {
            return targetCharacterId;
        }

        public bool isTargetedAt(Id<CharacterId> characterId) {
            return Equals(targetCharacterId, characterId);
        }

        public abstract CombatEventType getType();
    }

    public enum CombatEventType {
        DAMAGE_INCOMING,
        DAMAGE_RESOLVED,
        HEALTH_LOST
    }

    public enum DamageSourceType {
        EnemyAttack,
        SelfDamage,
        Sacrifice
    }
}