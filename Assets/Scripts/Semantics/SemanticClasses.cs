using System;
using UnityEngine;

namespace MageFactory.Semantics {
    public sealed class ItemsLayerRectTransform : Semantic<RectTransform> {
        public ItemsLayerRectTransform(RectTransform value) : base(value) {
        }
    }

    public abstract class Semantic<T> {
        private readonly T _value;

        protected Semantic(T value) {
            if (value is null) throw new ArgumentNullException(nameof(value));

            _value = value;
        }

        public T Get() {
            return _value;
        }

        public static implicit operator T(Semantic<T> s) {
            return s._value;
        }

        public override string ToString() {
            return $"{typeof(T).Name}: {_value}";
        }
    }
}