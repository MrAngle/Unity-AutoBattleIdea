using System;

namespace MageFactory.Shared.Id {
    public readonly struct Id<T> : IEquatable<Id<T>> {
        public long Value { get; }
        public Id(long value) => Value = value;

        public bool Equals(Id<T> other) => Value == other.Value;
        public override bool Equals(object obj) => obj is Id<T> other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Id<T> a, Id<T> b) => a.Value == b.Value;
        public static bool operator !=(Id<T> a, Id<T> b) => a.Value != b.Value;

        public override string ToString() => Value.ToString();
    }

    public readonly struct CharacterId {
    }

    public readonly struct ItemId {
    }
}