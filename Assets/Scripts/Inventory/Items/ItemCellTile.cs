using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Items {
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class ItemCellTile : MonoBehaviour {
        [SerializeField] private Image _image;

        private void Awake() {
            if (!_image) _image = GetComponent<Image>();
            _image.raycastTarget = true; // na razie bez interakcji
        }

        public void SetupVisual(Color color) {
            if (!_image) _image = GetComponent<Image>();
            _image.color = color;
        }

        public void SetSize(Vector2 size) {
            var rt = (RectTransform)transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}