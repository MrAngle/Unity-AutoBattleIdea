using System;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar {
    internal sealed class PreparedGuard {
        private readonly Id<GuardId> guardId;
        private long power;

        internal PreparedGuard(Id<GuardId> guardId, GuardPower guardPower) {
            this.guardId = NullGuard.ValidIdOrThrow(guardId);
            power = validatePositivePower(NullGuard.NotNullOrThrow(guardPower).getPower());
        }

        internal Id<GuardId> getGuardId() {
            return guardId;
        }

        internal long getPower() {
            return power;
        }

        internal bool hasNoPower() {
            return power <= 0;
        }

        internal long absorb(long incomingDamage) {
            if (incomingDamage < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(incomingDamage),
                    incomingDamage,
                    "Incoming damage cannot be negative.");
            }

            if (incomingDamage == 0 || power == 0) {
                return 0;
            }

            long blockedDamage = GuardMitigationCalculator.calculateBlockedDamage(power, incomingDamage);
            power = Math.Max(0, power - blockedDamage);
            return blockedDamage;
        }

        internal PreparedGuardState toState() {
            return new PreparedGuardState(guardId, new GuardPower(power));
        }

        private static long validatePositivePower(long power) {
            if (power <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(power),
                    power,
                    "Prepared guard must have positive power.");
            }

            return power;
        }
    }
}