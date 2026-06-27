using System;

namespace MageFactory.Shared.Model {
    public sealed class StabilityPower : PowerAmount {
        public StabilityPower(long power) : base(validatePower(power)) {
        }

        public new static StabilityPower noPower() {
            return new StabilityPower(0);
        }

        public static StabilityPower fromPowerAmount(PowerAmount power) {
            if (power == null) {
                throw new ArgumentNullException(nameof(power));
            }

            return new StabilityPower(Math.Max(0, power.getPower()));
        }

        private static long validatePower(long power) {
            if (power < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(power),
                    power,
                    "Stability power cannot be negative.");
            }

            return power;
        }
    }

    public static class StabilityMitigationCalculator {
        public const long ReferenceStability = 100;
        public const int DefaultStrainNumerator = 1;
        public const int DefaultStrainDenominator = 3;
        public const long OverBaselineDecayUnitScale = 10_000;
        public const int OverBaselineDecayThresholdNumerator = 11;
        public const int OverBaselineDecayThresholdDenominator = 10;
        public const int OverBaselineDecayAtFullThresholdExcess = 10;
        public const int OverBaselineDecayCurvePower = 3;

        public static long calculateDamageAfterStability(long stability, long incomingDamage) {
            return calculateDamageAfterStability(stability, ReferenceStability, incomingDamage);
        }

        public static long calculateDamageAfterStability(
            long stability,
            long baselineStability,
            long incomingDamage) {
            if (incomingDamage < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(incomingDamage),
                    incomingDamage,
                    "Incoming damage cannot be negative.");
            }

            if (baselineStability <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(baselineStability),
                    baselineStability,
                    "Baseline stability must be positive.");
            }

            if (incomingDamage == 0) {
                return 0;
            }

            if (stability >= 0) {
                long denominator = stability > long.MaxValue - baselineStability
                    ? long.MaxValue
                    : baselineStability + stability;
                long roundedDamage = incomingDamage <= long.MaxValue / baselineStability
                    ? incomingDamage * baselineStability / denominator
                    : (long)Math.Floor(incomingDamage * ((double)baselineStability / denominator));
                return Math.Max(1, Math.Min(incomingDamage, roundedDamage));
            }

            long vulnerability = -stability;
            double bonusDamage = incomingDamage * ((double)vulnerability / baselineStability);
            long roundedBonusDamage = (long)Math.Floor(bonusDamage);

            if (long.MaxValue - incomingDamage < roundedBonusDamage) {
                return long.MaxValue;
            }

            return incomingDamage + Math.Max(0, roundedBonusDamage);
        }

        public static long calculateReducedDamage(long stability, long incomingDamage) {
            long damageAfterStability = calculateDamageAfterStability(stability, incomingDamage);
            return Math.Max(0, incomingDamage - damageAfterStability);
        }

        public static long calculateReducedDamage(
            long stability,
            long baselineStability,
            long incomingDamage) {
            long damageAfterStability = calculateDamageAfterStability(
                stability,
                baselineStability,
                incomingDamage);
            return Math.Max(0, incomingDamage - damageAfterStability);
        }

        public static long calculateDefaultStrain(long incomingDamage) {
            if (incomingDamage < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(incomingDamage),
                    incomingDamage,
                    "Incoming damage cannot be negative.");
            }

            return incomingDamage * DefaultStrainNumerator / DefaultStrainDenominator;
        }

        public static long calculateOverBaselineDecay(long stability, long baselineStability) {
            return calculateOverBaselineDecayUnits(stability, baselineStability) / OverBaselineDecayUnitScale;
        }

        public static long calculateOverBaselineDecayUnits(long stability, long baselineStability) {
            if (baselineStability <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(baselineStability),
                    baselineStability,
                    "Baseline stability must be positive.");
            }

            long decayThreshold = calculateOverBaselineDecayThreshold(baselineStability);
            if (stability <= decayThreshold) {
                return 0;
            }

            long excessAboveThreshold = stability - decayThreshold;
            double normalizedExcess = excessAboveThreshold / (double)Math.Max(1, decayThreshold);
            double decayPerTick = Math.Pow(normalizedExcess, OverBaselineDecayCurvePower)
                                  * OverBaselineDecayAtFullThresholdExcess;

            if (decayPerTick <= 0) {
                return 0;
            }

            if (decayPerTick >= long.MaxValue / (double)OverBaselineDecayUnitScale) {
                return long.MaxValue;
            }

            return Math.Max(1, (long)Math.Floor(decayPerTick * OverBaselineDecayUnitScale));
        }

        private static long calculateOverBaselineDecayThreshold(long baselineStability) {
            if (baselineStability > long.MaxValue / OverBaselineDecayThresholdNumerator) {
                return long.MaxValue;
            }

            return divideCeiling(
                baselineStability * OverBaselineDecayThresholdNumerator,
                OverBaselineDecayThresholdDenominator);
        }

        private static long divideCeiling(long value, long divisor) {
            return value / divisor + (value % divisor == 0 ? 0 : 1);
        }
    }
}