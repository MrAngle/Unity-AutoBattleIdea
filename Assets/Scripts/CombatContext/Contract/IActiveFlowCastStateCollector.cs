using System;
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

    public interface IActiveFlowCastStateCollector {
        void addActiveFlowCastState(ActiveFlowCastState castState);
    }
}