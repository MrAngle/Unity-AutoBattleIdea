using System;
using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Contract {
    public readonly struct ActiveFlowCastState {
        private readonly ItemFlowProcessingSlot processingSlot;
        private readonly CombatTicks remainingCastTicks;
        private readonly CombatTicks requiredCastTicks;

        public ActiveFlowCastState(
            ItemFlowProcessingSlot processingSlot,
            CombatTicks remainingCastTicks,
            CombatTicks requiredCastTicks) {
            this.processingSlot = NullGuard.NotNullOrThrow(processingSlot);

            if (remainingCastTicks.isNegative()) {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingCastTicks),
                    remainingCastTicks,
                    "Remaining cast ticks cannot be negative.");
            }

            if (requiredCastTicks.isNegative()) {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredCastTicks),
                    requiredCastTicks,
                    "Required cast ticks cannot be negative.");
            }

            if (remainingCastTicks > requiredCastTicks) {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingCastTicks),
                    remainingCastTicks,
                    "Remaining cast ticks cannot be greater than required cast ticks.");
            }

            this.remainingCastTicks = remainingCastTicks;
            this.requiredCastTicks = requiredCastTicks;
        }

        public Id<ItemId> getItemId() {
            return processingSlot.getItemId();
        }

        public ItemFlowProcessingSlot getProcessingSlot() {
            return processingSlot;
        }

        public CombatTicks getRemainingCastTicks() {
            return remainingCastTicks;
        }

        public CombatTicks getRequiredCastTicks() {
            return requiredCastTicks;
        }
    }

    public readonly struct ActiveFlowState {
        private readonly Id<ActiveFlowId> flowId;
        private readonly FlowKind flowKind;
        private readonly IReadOnlyList<ItemFlowProcessingSlot> processingPath;
        private readonly ActiveFlowCastState castState;

        public ActiveFlowState(
            Id<ActiveFlowId> flowId,
            FlowKind flowKind,
            IReadOnlyList<ItemFlowProcessingSlot> processingPath,
            ActiveFlowCastState castState) {
            this.flowId = NullGuard.ValidIdOrThrow(flowId);
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.processingPath = NullGuard.NotNullOrThrow(processingPath);

            if (this.processingPath.Count == 0) {
                throw new ArgumentException("Active flow path cannot be empty.", nameof(processingPath));
            }

            for (int i = 0; i < this.processingPath.Count; i++) {
                NullGuard.NotNullOrThrow(this.processingPath[i]);
            }

            this.castState = castState;
        }

        public Id<ActiveFlowId> getFlowId() {
            return flowId;
        }

        public FlowKind getFlowKind() {
            return flowKind;
        }

        public IReadOnlyList<ItemFlowProcessingSlot> getProcessingPath() {
            return processingPath;
        }

        public ActiveFlowCastState getCastState() {
            return castState;
        }

        public bool tryGetPreviousProcessingSlot(out ItemFlowProcessingSlot previousProcessingSlot) {
            if (processingPath.Count < 2) {
                previousProcessingSlot = null;
                return false;
            }

            previousProcessingSlot = processingPath[processingPath.Count - 2];
            return true;
        }
    }

    public interface IActiveFlowStateCollector {
        void addActiveFlowState(ActiveFlowState flowState);
    }

    public readonly struct PreparedGuardState {
        private readonly Id<GuardId> guardId;
        private readonly GuardPower guardPower;

        public PreparedGuardState(Id<GuardId> guardId, GuardPower guardPower) {
            this.guardId = NullGuard.ValidIdOrThrow(guardId);
            this.guardPower = NullGuard.NotNullOrThrow(guardPower);

            if (this.guardPower.getPower() <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(guardPower),
                    this.guardPower.getPower(),
                    "Visible prepared guard must have positive power.");
            }
        }

        public Id<GuardId> getGuardId() {
            return guardId;
        }

        public GuardPower getGuardPower() {
            return guardPower;
        }
    }

    public interface IPreparedGuardStateCollector {
        void addPreparedGuardState(PreparedGuardState guardState);
    }

    public readonly struct PreparedGuardAddResult {
        private readonly bool guardAdded;
        private readonly PreparedGuardState addedGuardState;
        private readonly bool guardReplaced;
        private readonly PreparedGuardState replacedGuardState;

        public PreparedGuardAddResult(
            PreparedGuardState addedGuardState,
            bool guardReplaced,
            PreparedGuardState replacedGuardState) {
            this.guardAdded = true;
            this.addedGuardState = addedGuardState;
            this.guardReplaced = guardReplaced;
            this.replacedGuardState = replacedGuardState;
        }

        public bool hasAddedGuard() {
            return guardAdded;
        }

        public PreparedGuardState getAddedGuardState() {
            if (!guardAdded) {
                throw new InvalidOperationException("Guard add result has no added guard.");
            }

            return addedGuardState;
        }

        public bool hasReplacedGuard() {
            return guardReplaced;
        }

        public PreparedGuardState getReplacedGuardState() {
            if (!guardReplaced) {
                throw new InvalidOperationException("Guard add result has no replaced guard.");
            }

            return replacedGuardState;
        }
    }

    public readonly struct StabilityPowerAddResult {
        private readonly bool stabilityAdded;
        private readonly long addedStabilityPower;
        private readonly long stabilityBefore;
        private readonly long stabilityAfter;
        private readonly long baselineStability;

        public StabilityPowerAddResult(
            long addedStabilityPower,
            long stabilityBefore,
            long stabilityAfter,
            long baselineStability) {
            if (addedStabilityPower <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(addedStabilityPower),
                    addedStabilityPower,
                    "Added stability power must be positive.");
            }

            if (baselineStability <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(baselineStability),
                    baselineStability,
                    "Baseline stability must be positive.");
            }

            this.stabilityAdded = true;
            this.addedStabilityPower = addedStabilityPower;
            this.stabilityBefore = stabilityBefore;
            this.stabilityAfter = stabilityAfter;
            this.baselineStability = baselineStability;
        }

        public bool hasAddedStability() {
            return stabilityAdded;
        }

        public long getAddedStabilityPower() {
            if (!stabilityAdded) {
                throw new InvalidOperationException("Stability add result has no added stability.");
            }

            return addedStabilityPower;
        }

        public long getStabilityBefore() {
            if (!stabilityAdded) {
                throw new InvalidOperationException("Stability add result has no added stability.");
            }

            return stabilityBefore;
        }

        public long getStabilityAfter() {
            if (!stabilityAdded) {
                throw new InvalidOperationException("Stability add result has no added stability.");
            }

            return stabilityAfter;
        }

        public long getBaselineStability() {
            if (!stabilityAdded) {
                throw new InvalidOperationException("Stability add result has no added stability.");
            }

            return baselineStability;
        }
    }
}