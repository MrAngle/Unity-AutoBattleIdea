using UnityEngine;

namespace Config.Semantics {
    public sealed class ItemsLayerRectTransform {
        public RectTransform Get() {
            return _value;
        }
        
        private RectTransform _value;
        public ItemsLayerRectTransform(RectTransform value) => _value = value;
    }

    
    public class SemanticClasses {
        
    }
}