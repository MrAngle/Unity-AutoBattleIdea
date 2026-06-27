using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar {
    internal sealed class CombatStabilityState {
        private long currentStability = StabilityMitigationCalculator.ReferenceStability;
        private long overBaselineDecayUnitsRemainder;

        internal void applyOverBaselineDecay(CombatTicks combatTicks) {
            CombatTicks ticks = combatTicks;
            if (!ticks.isPositive()) {
                return;
            }

            for (int i = 0; i < ticks.getValue(); i++) {
                long decayUnits = StabilityMitigationCalculator.calculateOverBaselineDecayUnits(
                    currentStability,
                    getBaselineStability());
                if (decayUnits <= 0) {
                    overBaselineDecayUnitsRemainder = 0;
                    return;
                }

                overBaselineDecayUnitsRemainder = addWithoutOverflow(
                    overBaselineDecayUnitsRemainder,
                    decayUnits);

                long decay = overBaselineDecayUnitsRemainder
                             / StabilityMitigationCalculator.OverBaselineDecayUnitScale;
                if (decay <= 0) {
                    continue;
                }

                overBaselineDecayUnitsRemainder -= decay
                                                   * StabilityMitigationCalculator.OverBaselineDecayUnitScale;
                currentStability = currentStability > decay
                    ? currentStability - decay
                    : 0;
            }
        }

        internal bool tryAddStability(
            StabilityPower stabilityPower,
            out StabilityPowerAddResult stabilityAddResult) {
            StabilityPower powerToAdd = NullGuard.NotNullOrThrow(stabilityPower);

            if (powerToAdd.getPower() <= 0) {
                stabilityAddResult = default;
                return false;
            }

            long stabilityBefore = currentStability;
            currentStability = addWithoutOverflow(currentStability, powerToAdd.getPower());

            stabilityAddResult = new StabilityPowerAddResult(
                powerToAdd.getPower(),
                stabilityBefore,
                currentStability,
                getBaselineStability());
            return true;
        }

        internal ResolvedDamage applyTo(
            ResolvedDamage resolvedDamage,
            out StabilityDamageApplicationResult stabilityDamageApplicationResult) {
            ResolvedDamage incomingDamage = NullGuard.NotNullOrThrow(resolvedDamage);
            long incomingPower = incomingDamage.getPower();

            if (incomingPower <= 0) {
                stabilityDamageApplicationResult = new StabilityDamageApplicationResult(
                    incomingPower,
                    incomingPower,
                    0,
                    currentStability,
                    currentStability,
                    getBaselineStability());
                return incomingDamage;
            }

            long stabilityBefore = currentStability;
            long remainingDamage = StabilityMitigationCalculator.calculateDamageAfterStability(
                stabilityBefore,
                getBaselineStability(),
                incomingPower);
            long strainedStability = StabilityMitigationCalculator.calculateDefaultStrain(incomingPower);
            currentStability = applyOrdinaryStrain(currentStability, strainedStability);

            stabilityDamageApplicationResult = new StabilityDamageApplicationResult(
                incomingPower,
                remainingDamage,
                strainedStability,
                stabilityBefore,
                currentStability,
                getBaselineStability());
            return new ResolvedDamage(remainingDamage);
        }

        internal long getCurrentStability() {
            return currentStability;
        }

        internal long getBaselineStability() {
            return StabilityMitigationCalculator.ReferenceStability;
        }

        private static long applyOrdinaryStrain(long stability, long strain) {
            if (strain <= 0) {
                return stability;
            }

            if (stability <= 0) {
                return stability;
            }

            return stability > strain
                ? stability - strain
                : 0;
        }

        private static long addWithoutOverflow(long current, long amount) {
            if (amount > 0 && current > long.MaxValue - amount) {
                return long.MaxValue;
            }

            return current + amount;
        }
    }

    internal readonly struct StabilityDamageApplicationResult {
        private readonly long incomingDamage;
        private readonly long remainingDamage;
        private readonly long stabilityStrain;
        private readonly long stabilityBefore;
        private readonly long stabilityAfter;
        private readonly long baselineStability;

        internal StabilityDamageApplicationResult(
            long incomingDamage,
            long remainingDamage,
            long stabilityStrain,
            long stabilityBefore,
            long stabilityAfter,
            long baselineStability) {
            this.incomingDamage = incomingDamage;
            this.remainingDamage = remainingDamage;
            this.stabilityStrain = stabilityStrain;
            this.stabilityBefore = stabilityBefore;
            this.stabilityAfter = stabilityAfter;
            this.baselineStability = baselineStability;
        }

        internal bool hasChangedDamageOrStability() {
            return getReducedDamage() > 0 || stabilityStrain > 0 || stabilityBefore != stabilityAfter;
        }

        internal long getIncomingDamage() {
            return incomingDamage;
        }

        internal long getRemainingDamage() {
            return remainingDamage;
        }

        internal long getReducedDamage() {
            return incomingDamage > remainingDamage
                ? incomingDamage - remainingDamage
                : 0;
        }

        internal long getStabilityStrain() {
            return stabilityStrain;
        }

        internal long getStabilityBefore() {
            return stabilityBefore;
        }

        internal long getStabilityAfter() {
            return stabilityAfter;
        }

        internal long getBaselineStability() {
            return baselineStability;
        }
    }
}