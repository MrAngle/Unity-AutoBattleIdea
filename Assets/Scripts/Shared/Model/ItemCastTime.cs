using System;
using MageFactory.Shared.Utility;

namespace MageFactory.Shared.Model {
    public sealed class ItemCastTime : IEquatable<ItemCastTime> {
        public static readonly ItemCastTime ZERO = new(CombatTicks.ZERO);

        private readonly CombatTicks ticks;

        private ItemCastTime(CombatTicks ticks) {
            if (ticks.isNegative()) {
                throw new ArgumentOutOfRangeException(
                    nameof(ticks),
                    ticks,
                    "Item cast time cannot be negative.");
            }

            this.ticks = ticks;
        }

        public static ItemCastTime ofTicks(CombatTicks ticks) {
            return new ItemCastTime(ticks);
        }

        public static ItemCastTime ofTicks(int ticks) {
            return new ItemCastTime(CombatTicks.of(ticks));
        }

        public CombatTicks getTicks() {
            return ticks;
        }

        public CombatTicks getCastTicksFor(ItemFlowProcessingSlot processingSlot) {
            ItemFlowProcessingSlot slot = NullGuard.NotNullOrThrow(processingSlot);
            int castTicksForSlot = checked(ticks.getValue() * slot.getCellCount());
            return CombatTicks.of(castTicksForSlot);
        }

        public bool isInstant() {
            return ticks.isZero();
        }

        public bool Equals(ItemCastTime other) {
            if (other is null) {
                return false;
            }

            return ticks.Equals(other.ticks);
        }

        public override bool Equals(object obj) {
            return obj is ItemCastTime other && Equals(other);
        }

        public override int GetHashCode() {
            return ticks.GetHashCode();
        }

        public static bool operator ==(ItemCastTime left, ItemCastTime right) {
            if (ReferenceEquals(left, right)) {
                return true;
            }

            if (left is null || right is null) {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ItemCastTime left, ItemCastTime right) {
            return !(left == right);
        }

        public override string ToString() {
            return ticks.ToString();
        }
    }
}