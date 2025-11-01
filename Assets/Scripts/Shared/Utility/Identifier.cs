// Shared/Identifier/StrongIds.cs
using System;
using System.Runtime.CompilerServices;

namespace Shared.Utility
{
    /// Wspólna baza (bez publicznych fabryk/parsers).
    public abstract class StrongId<TSelf> :
        IEquatable<TSelf>, IComparable<TSelf>
        where TSelf : StrongId<TSelf>, new()
    {
        public long Value { get; private set; }
        public bool IsEmpty => Value == 0;

        protected StrongId() { }
        protected StrongId(long value) { Value = value; }

        // Jedyny "rdzeń" do ustawiania Value – dostępny dla potomnych klas/polityk.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static TSelf FromCore(long value) => new TSelf { Value = value };

        // Równość/porządkowanie po Value
        public bool Equals(TSelf other) => !(other is null) && Value == other.Value;
        public int CompareTo(TSelf other) => other is null ? 1 : Value.CompareTo(other.Value);
        public override bool Equals(object obj) => obj is TSelf other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(StrongId<TSelf> a, StrongId<TSelf> b)
            => ReferenceEquals(a, b) || (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Value == b.Value);
        public static bool operator !=(StrongId<TSelf> a, StrongId<TSelf> b) => !(a == b);
        public static bool operator < (StrongId<TSelf> a, StrongId<TSelf> b) => a.Value <  b.Value;
        public static bool operator > (StrongId<TSelf> a, StrongId<TSelf> b) => a.Value >  b.Value;
        public static bool operator <=(StrongId<TSelf> a, StrongId<TSelf> b) => a.Value <= b.Value;
        public static bool operator >=(StrongId<TSelf> a, StrongId<TSelf> b) => a.Value >= b.Value;

        public static explicit operator long(StrongId<TSelf> id) => id.Value;

        public override string ToString() => $"{typeof(TSelf).Name}:{Value}";
    }

    /// Polityka: ID tworzone w runtime (publiczne fabryki i parsowanie).
    public abstract class RuntimeId<TSelf> : StrongId<TSelf>
        where TSelf : RuntimeId<TSelf>, new()
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSelf From(long value) => FromCore(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSelf New() => FromCore(IdGenerator.Next());

        public static bool TryParse(ReadOnlySpan<char> text, out TSelf id)
        {
            int colon = text.LastIndexOf(':');
            if (colon >= 0 && colon + 1 < text.Length) text = text[(colon + 1)..];
            if (long.TryParse(text, out var v)) { id = From(v); return true; }
            id = null; return false;
        }

        public static TSelf Parse(string text)
        {
            if (TryParse(text.AsSpan(), out var id)) return id;
            throw new FormatException($"Cannot parse {typeof(TSelf).Name} from '{text}'.");
        }
    }

    /// Polityka: stałe ID – brak publicznych fabryk/parsers.
    /// Tworzysz WYŁĄCZNIE własne stałe w typie pochodnym.
    public abstract class ConstantId<TSelf> : StrongId<TSelf>
        where TSelf : ConstantId<TSelf>, new()
    {
        // Pomocnik tylko dla pochodnych przy definiowaniu stałych:
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static TSelf Define(long value) => FromCore(value);

        // Świadomie brak: public From/New/TryParse/Parse.
        // Dzięki temu nikt z zewnątrz nie utworzy nowej instancji.
    }
}
