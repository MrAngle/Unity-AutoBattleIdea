using MageFactory.Shared.Id;

namespace MageFactory.Character.Domain.CombatChar {
    internal readonly struct GuardDamageApplicationResult {
        private readonly long incomingDamage;
        private readonly long blockedDamage;
        private readonly long remainingDamage;
        private readonly int destroyedGuardCount;
        private readonly long remainingGuardPower;
        private readonly bool hasAffectedGuard;
        private readonly Id<GuardId> firstAffectedGuardId;

        internal GuardDamageApplicationResult(
            long incomingDamage,
            long blockedDamage,
            long remainingDamage,
            int destroyedGuardCount,
            long remainingGuardPower,
            bool hasAffectedGuard,
            Id<GuardId> firstAffectedGuardId) {
            this.incomingDamage = incomingDamage;
            this.blockedDamage = blockedDamage;
            this.remainingDamage = remainingDamage;
            this.destroyedGuardCount = destroyedGuardCount;
            this.remainingGuardPower = remainingGuardPower;
            this.hasAffectedGuard = hasAffectedGuard;
            this.firstAffectedGuardId = firstAffectedGuardId;
        }

        internal long getIncomingDamage() {
            return incomingDamage;
        }

        internal long getBlockedDamage() {
            return blockedDamage;
        }

        internal long getRemainingDamage() {
            return remainingDamage;
        }

        internal int getDestroyedGuardCount() {
            return destroyedGuardCount;
        }

        internal long getRemainingGuardPower() {
            return remainingGuardPower;
        }

        internal bool hasFirstAffectedGuard() {
            return hasAffectedGuard;
        }

        internal Id<GuardId> getFirstAffectedGuardId() {
            return firstAffectedGuardId;
        }

        internal bool hasBlockedDamage() {
            return blockedDamage > 0;
        }
    }
}