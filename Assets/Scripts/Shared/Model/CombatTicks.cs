using System;

namespace MageFactory.Shared.Model {
    public readonly struct CombatTicks : IEquatable<CombatTicks>, IComparable<CombatTicks> {
        public static readonly CombatTicks ZERO = new(0);
        public static readonly CombatTicks ONE = new(1);

        private readonly int value;

        private CombatTicks(int value) {
            this.value = value;
        }

        public static CombatTicks of(int value) {
            return new CombatTicks(value);
        }

        public int getValue() {
            return value;
        }

        public bool isZero() {
            return value == 0;
        }

        public bool isPositive() {
            return value > 0;
        }

        public bool isNegative() {
            return value < 0;
        }

        public CombatTicks abs() {
            return value < 0
                ? new CombatTicks(-value)
                : this;
        }

        public int CompareTo(CombatTicks other) {
            return value.CompareTo(other.value);
        }

        public bool Equals(CombatTicks other) {
            return value == other.value;
        }

        public override bool Equals(object obj) {
            return obj is CombatTicks other && Equals(other);
        }

        public override int GetHashCode() {
            return value;
        }

        public static CombatTicks operator +(CombatTicks left, CombatTicks right) {
            return new CombatTicks(left.value + right.value);
        }

        public static CombatTicks operator -(CombatTicks left, CombatTicks right) {
            return new CombatTicks(left.value - right.value);
        }

        public static CombatTicks operator -(CombatTicks value) {
            return new CombatTicks(-value.value);
        }

        public static CombatTicks operator %(CombatTicks left, CombatTicks right) {
            if (right.value == 0) {
                throw new DivideByZeroException("Cannot calculate modulo by zero CombatTicks.");
            }

            return new CombatTicks(left.value % right.value);
        }

        public static bool operator ==(CombatTicks left, CombatTicks right) {
            return left.Equals(right);
        }

        public static bool operator !=(CombatTicks left, CombatTicks right) {
            return !left.Equals(right);
        }

        public static bool operator >(CombatTicks left, CombatTicks right) {
            return left.value > right.value;
        }

        public static bool operator <(CombatTicks left, CombatTicks right) {
            return left.value < right.value;
        }

        public static bool operator >=(CombatTicks left, CombatTicks right) {
            return left.value >= right.value;
        }

        public static bool operator <=(CombatTicks left, CombatTicks right) {
            return left.value <= right.value;
        }

        public override string ToString() {
            return $"{value} tick(s)";
        }
    }
}