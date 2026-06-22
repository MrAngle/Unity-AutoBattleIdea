using System;

namespace MageFactory.Shared.Model {
    public sealed class GuardPower : PowerAmount {
        public GuardPower(long power) : base(validatePower(power)) {
        }

        public new static GuardPower noPower() {
            return new GuardPower(0);
        }

        public static GuardPower fromPowerAmount(PowerAmount power) {
            if (power == null) {
                throw new ArgumentNullException(nameof(power));
            }

            return new GuardPower(Math.Max(0, power.getPower()));
        }

        private static long validatePower(long power) {
            if (power < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(power),
                    power,
                    "Guard power cannot be negative.");
            }

            return power;
        }
    }

    public static class GuardMitigationCalculator {
        public static long calculateBlockedDamage(GuardPower guardPower, long incomingDamage) {
            if (guardPower == null) {
                throw new ArgumentNullException(nameof(guardPower));
            }

            return calculateBlockedDamage(guardPower.getPower(), incomingDamage);
        }

        public static long calculateBlockedDamage(long guardPower, long incomingDamage) {
            if (guardPower < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(guardPower),
                    guardPower,
                    "Guard power cannot be negative.");
            }

            if (incomingDamage < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(incomingDamage),
                    incomingDamage,
                    "Incoming damage cannot be negative.");
            }

            if (guardPower == 0 || incomingDamage == 0) {
                return 0;
            }

            double incoming = incomingDamage;
            double guard = guardPower;
            double incomingShare = incoming / (incoming + guard);
            long blockedDamage = (long)Math.Floor(incoming * (1d - incomingShare * incomingShare));

            if (blockedDamage >= incomingDamage) {
                return incomingDamage - 1;
            }

            return Math.Max(0, blockedDamage);
        }
    }
}