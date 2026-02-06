using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Inventory.Api {
    public class ItemView : MonoBehaviour {
        [Header("Visual")] [SerializeField] private Color cellColor = new(0.4f, 0.7f, 1f, 0.85f);

        [SerializeField] private float cellSpacing = 2f; // odstęp między kafelkami (px)
        private readonly IPlacedItem _placedItem;

        private readonly List<ItemCellTileView> _tiles = new();
        private Vector2 _cellSize;

        private Vector2Int[] _shapeOffsets;

        public void Build(ShapeArchetype data, Vector2 cellSize) {
            // _placedItem = IPlacedItem.CreateBattleItem(data); 

            Clear();
            _cellSize = cellSize;
            _shapeOffsets = data.Shape.Cells.ToArray();

            foreach (var offset in _shapeOffsets) {
                var go = new GameObject($"Tile_{offset.x}_{offset.y}",
                    typeof(RectTransform), typeof(Image), typeof(ItemCellTileView));
                go.transform.SetParent(transform, false);

                var tile = go.GetComponent<ItemCellTileView>();
                tile.SetupVisual(cellColor);
                tile.SetSize(_cellSize);

                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(
                    offset.x * (_cellSize.x + cellSpacing),
                    -offset.y * (_cellSize.y + cellSpacing)
                );

                _tiles.Add(tile);
            }

            ResizeToFit();
        }

        /// Ustaw pozycję origin (x,y) względem lewego-górnego rogu gridu (gridOrigin).
        public void SetOriginInGrid(Vector2Int origin, Vector2 cellSize, Vector2 gridOrigin, float spacing = 0f) {
            _cellSize = cellSize; // gdyby grid używał innego cellSize niż w Build
            var rt = (RectTransform)transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            var x = origin.x * (_cellSize.x + spacing);
            var y = -origin.y * (_cellSize.y + spacing);
            rt.anchoredPosition = gridOrigin + new Vector2(x, y);
        }

        public void SetColor(Color c) {
            foreach (var t in _tiles) t.SetupVisual(c);
        }

        public void Clear() {
            foreach (Transform child in transform) Destroy(child.gameObject);
            _tiles.Clear();
        }

        private void ResizeToFit() {
            if (_shapeOffsets == null || _shapeOffsets.Length == 0) return;
            int minX = _shapeOffsets.Min(o => o.x), maxX = _shapeOffsets.Max(o => o.x);
            int minY = _shapeOffsets.Min(o => o.y), maxY = _shapeOffsets.Max(o => o.y);

            int wCells = maxX - minX + 1, hCells = maxY - minY + 1;
            var w = wCells * _cellSize.x + (wCells - 1) * cellSpacing;
            var h = hCells * _cellSize.y + (hCells - 1) * cellSpacing;

            var rt = (RectTransform)transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }
    }
}