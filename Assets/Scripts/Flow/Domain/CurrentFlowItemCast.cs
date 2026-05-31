using System;
using MageFactory.ActionEffect;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal sealed class CurrentFlowItemCast {
        private CombatTicks ticksUntilCastCompletes;
        private CombatTicks requiredCastTicks;

        internal void startCasting(
            IFlowItem item,
            FlowCastTimeMode castTimeMode,
            ItemFlowProcessingSlot processingSlot) {
            IFlowItem castItem = NullGuard.NotNullOrThrow(item);
            ItemFlowProcessingSlot slot = NullGuard.NotNullOrThrow(processingSlot);

            if (castTimeMode == FlowCastTimeMode.Instant) {
                startCastingFor(CombatTicks.ZERO);
                return;
            }

            IActionDescription actionDescription = NullGuard.NotNullOrThrow(
                castItem.prepareItemActionDescription());
            ItemCastTime castTime = NullGuard.NotNullOrThrow(actionDescription.getCastTime());
            startCastingFor(castTime.getCastTicksFor(slot));
        }

        private void startCastingFor(CombatTicks ticksUntilCastCompletes) {
            if (ticksUntilCastCompletes.isNegative()) {
                throw new ArgumentOutOfRangeException(
                    nameof(ticksUntilCastCompletes),
                    ticksUntilCastCompletes,
                    "Current flow item cast cannot start with negative ticks.");
            }

            this.ticksUntilCastCompletes = ticksUntilCastCompletes;
            requiredCastTicks = ticksUntilCastCompletes;
        }

        internal bool tryFinishCasting(ref CombatTicks availableTicks) {
            if (isReadyToExecute()) {
                return true;
            }

            if (hasNoTicksToCastWith(availableTicks)) {
                return false;
            }

            if (canCompleteCastWith(availableTicks)) {
                finishCastingAndKeepUnusedTicks(ref availableTicks);
                return true;
            }

            spendAllTicksOnCasting(ref availableTicks);
            return false;
        }

        internal bool isCasting() {
            return ticksUntilCastCompletes > CombatTicks.ZERO;
        }

        internal bool hasMeasurableCastTime() {
            return requiredCastTicks > CombatTicks.ZERO;
        }

        internal CombatTicks getRemainingCastTicks() {
            return ticksUntilCastCompletes > CombatTicks.ZERO
                ? ticksUntilCastCompletes
                : CombatTicks.ZERO;
        }

        internal CombatTicks getRequiredCastTicks() {
            return requiredCastTicks;
        }

        private bool isReadyToExecute() {
            return ticksUntilCastCompletes <= CombatTicks.ZERO;
        }

        private static bool hasNoTicksToCastWith(CombatTicks availableTicks) {
            return availableTicks <= CombatTicks.ZERO;
        }

        private bool canCompleteCastWith(CombatTicks availableTicks) {
            return availableTicks >= ticksUntilCastCompletes;
        }

        private void finishCastingAndKeepUnusedTicks(ref CombatTicks availableTicks) {
            availableTicks -= ticksUntilCastCompletes;
            ticksUntilCastCompletes = CombatTicks.ZERO;
        }

        private void spendAllTicksOnCasting(ref CombatTicks availableTicks) {
            ticksUntilCastCompletes -= availableTicks;
            availableTicks = CombatTicks.ZERO;
        }
    }
}