using MageFactory.Shared.Model;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.GridLayer {
    public class InventoryCellView : MonoBehaviour {
        [SerializeField] private Image _image;

        public void Init(Vector2Int coord, CellState state) {
            if (_image == null) _image = GetComponent<Image>();
            ApplyStateVisual(state);
            gameObject.name = $"Cell_{coord.x}_{coord.y}";
        }

        public void ApplyStateVisual(CellState state) {
            if (_image == null) _image = GetComponent<Image>();
            _image.color = state switch {
                // CellState.Empty    => new Color(0.85f, 0.85f, 0.85f, 1f),
                CellState.Empty => new Color(0.5f, 0.9f, 0.9f, 1f),
                CellState.Unreachable => new Color(0.25f, 0.25f, 0.25f, 1f),
                CellState.Occupied => new Color(0.8f, 0.1f, 1f, 1f),
                _ => _image.color
            };
        }
    }
}