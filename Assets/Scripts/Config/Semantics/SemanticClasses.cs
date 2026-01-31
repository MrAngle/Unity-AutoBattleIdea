using System;
using Contracts.Inventory;
using Contracts.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Semantics {
    public sealed class ItemsLayerRectTransform : Semantic<RectTransform> {
        public ItemsLayerRectTransform(RectTransform value) : base(value) {
        }
    }

    public sealed class ItemViewPrefabItemView : Semantic<ItemView> {
        public ItemViewPrefabItemView(ItemView value) : base(value) {
        }
    }

    public sealed class DragGhostPrefabItemView : Semantic<ItemView> {
        public DragGhostPrefabItemView(ItemView value) : base(value) {
        }
    }

    public sealed class InventoryGridLayoutGroup : Semantic<GridLayoutGroup> {
        public InventoryGridLayoutGroup(GridLayoutGroup value) : base(value) {
        }
    }

    public sealed class GridViewPrefabInventoryGridView : Semantic<InventoryGridView> {
        public GridViewPrefabInventoryGridView(InventoryGridView value) : base(value) {
        }
    }

    public sealed class CellViewPrefabInventoryCellView : Semantic<InventoryCellView> {
        public CellViewPrefabInventoryCellView(InventoryCellView value) : base(value) {
        }
    }

    public abstract class Semantic<T> {
        private readonly T _value;

        protected Semantic(T value) {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

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

    public class SemanticClasses {
    }
}