using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public class PlacedItemView : MonoBehaviour {
        [Header("Visual")] [SerializeField] private Color cellColor = new(0.4f, 0.7f, 1f, 0.85f);
        [SerializeField] private float cellSpacing = 5f;

        [Header("Animation")] [SerializeField] private float moveDuration = 0.2f;
        [SerializeField] private Ease moveEase = Ease.OutCubic;

        private readonly List<ItemCellTileView> itemCellTileViews = new();
        private Vector2 cellSize;
        private Vector2Int[] shapeOffsets;
        private Tween moveTween;

        public void build(ShapeArchetype data, Vector2 targetCellSize) {
            clear();
            cellSize = targetCellSize;
            shapeOffsets = data.Shape.Cells.ToArray();

            foreach (var offset in shapeOffsets) {
                var go = new GameObject(
                    $"Tile_{offset.x}_{offset.y}",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(ItemCellTileView));

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

        public void setOriginInGrid(
            Vector2Int origin,
            Vector2 paramCellSize,
            Vector2 gridOrigin,
            float spacing = 0f) {
            cellSize = paramCellSize;

            var rt = (RectTransform)transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            rt.anchoredPosition = calculateAnchoredPosition(origin, paramCellSize, gridOrigin, spacing);
        }

        public void animateMoveTo(Vector2 targetAnchoredPosition) {
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);

            killMoveTween();
            moveTween = rectTransform.DOAnchorPos(targetAnchoredPosition, moveDuration)
                .SetEase(moveEase);
        }

        public void setColor(Color c) {
            foreach (var itemCellTileView in itemCellTileViews) {
                itemCellTileView.setupVisual(c);
            }
        }

        private Vector2 calculateAnchoredPosition(
            Vector2Int origin,
            Vector2 paramCellSize,
            Vector2 gridOrigin,
            float spacing) {
            float x = origin.x * (paramCellSize.x + spacing);
            float y = -origin.y * (paramCellSize.y + spacing);
            return gridOrigin + new Vector2(x, y);
        }

        private void clear() {
            killMoveTween();

            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            itemCellTileViews.Clear();
        }

        private void resizeToFit() {
            if (shapeOffsets == null || shapeOffsets.Length == 0) {
                return;
            }

            int minX = shapeOffsets.Min(o => o.x);
            int maxX = shapeOffsets.Max(o => o.x);
            int minY = shapeOffsets.Min(o => o.y);
            int maxY = shapeOffsets.Max(o => o.y);

            int wCells = maxX - minX + 1;
            int hCells = maxY - minY + 1;

            float width = wCells * cellSize.x + (wCells - 1) * cellSpacing;
            float height = hCells * cellSize.y + (hCells - 1) * cellSpacing;

            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        private void killMoveTween() {
            if (moveTween != null && moveTween.IsActive()) {
                moveTween.Kill();
                moveTween = null;
            }
        }

        private void OnDestroy() {
            killMoveTween();
        }
    }
}