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
}