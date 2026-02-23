using System.Collections.Generic;
using System.Linq;
using MageFactory.Item.Controller;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Inventory.Controller {
    public class PlacedItemView : MonoBehaviour {
        [Header("Visual")] [SerializeField] private Color cellColor = new(0.4f, 0.7f, 1f, 0.85f);
        [SerializeField] private float cellSpacing = 5f; // odstęp między kafelkami (px)

        private readonly List<ItemCellTileView> itemCellTileViews = new();
        private Vector2 cellSize;
        private Vector2Int[] shapeOffsets;

        internal void build(ShapeArchetype data, Vector2 targetCellSize) {
            clear();
            cellSize = targetCellSize;
            shapeOffsets = data.Shape.Cells.ToArray();

            foreach (var offset in shapeOffsets) {
                var go = new GameObject($"Tile_{offset.x}_{offset.y}",
                    typeof(RectTransform), typeof(Image), typeof(ItemCellTileView));
                go.transform.SetParent(transform, false);

                var tile = go.GetComponent<ItemCellTileView>();
                tile.setupVisual(cellColor);
                tile.setSize(cellSize);

                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(
                    offset.x * (cellSize.x + cellSpacing),
                    -offset.y * (cellSize.y + cellSpacing)
                );

                itemCellTileViews.Add(tile);
            }

            resizeToFit();
        }

        internal void setOriginInGrid(Vector2Int origin, Vector2 paramCellSize, Vector2 gridOrigin,
                                      float spacing = 0f) {
            cellSize = paramCellSize;
            var rt = (RectTransform)transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            var x = origin.x * (cellSize.x + spacing);
            var y = -origin.y * (cellSize.y + spacing);
            rt.anchoredPosition = gridOrigin + new Vector2(x, y);
        }

        internal void setColor(Color c) {
            foreach (var itemCellTileView in itemCellTileViews) {
                itemCellTileView.setupVisual(c);
            }
        }

        private void clear() {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            itemCellTileViews.Clear();
        }

        private void resizeToFit() {
            if (shapeOffsets == null || shapeOffsets.Length == 0) {
                return;
            }

            int minX = shapeOffsets.Min(o => o.x), maxX = shapeOffsets.Max(o => o.x);
            int minY = shapeOffsets.Min(o => o.y), maxY = shapeOffsets.Max(o => o.y);

            int wCells = maxX - minX + 1, hCells = maxY - minY + 1;
            var width = wCells * cellSize.x + (wCells - 1) * cellSpacing;
            var height = hCells * cellSize.y + (hCells - 1) * cellSpacing;

            var rt = (RectTransform)transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}