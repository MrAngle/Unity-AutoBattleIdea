using Inventory.Items.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Config.Semantics {
    
    public sealed class ItemsLayerRectTransform : Semantic<RectTransform> {
        public ItemsLayerRectTransform(RectTransform value) : base(value) {}
    }
    
    public sealed class ItemViewPrefabItemView : Semantic<ItemView> {
        public ItemViewPrefabItemView(ItemView value) : base(value) {}
    }
    
    public sealed class DragGhostPrefabItemView : Semantic<ItemView> {
        public DragGhostPrefabItemView(ItemView value) : base(value) {}
    }
    
    public sealed class InventoryGridLayoutGroup : Semantic<GridLayoutGroup>
    {
        public InventoryGridLayoutGroup(GridLayoutGroup value) : base(value) {}
    }
    
    public abstract class Semantic<T>
    {
        public T Get() {
            return _value;
        }

        private T _value;

        protected Semantic(T value)
        {
            if (value is null) {
                throw new System.ArgumentNullException(nameof(value));
            }
            _value = value;
        }

        public static implicit operator T(Semantic<T> s) => s._value;

        public override string ToString() => $"{typeof(T).Name}: {_value}";
    }
    
    public class SemanticClasses {
        
    }
}