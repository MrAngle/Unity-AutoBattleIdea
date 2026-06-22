using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public class PlacedItemView : MonoBehaviour {
        [Header("Visual")] [SerializeField] private Color cellColor = new(0.4f, 0.7f, 1f, 0.85f);
        [SerializeField] private Vector2 cellSpacing = new(5f, 5f);

        [Header("Animation")] [SerializeField] private float moveDuration = 0.2f;
        [SerializeField] private Ease moveEase = Ease.OutCubic;

        private readonly List<ItemCellTileView> itemCellTileViews = new();
        private Vector2 cellSize;
        private Vector2Int[] shapeOffsets;
        private Tween moveTween;
        private ItemCastProgressBarsView castProgressBarsView;
        private FlowPortBadgeView flowPortBadgeView;
        private Action clickHandler;

        public void build(ShapeArchetype data, Vector2 targetCellSize) {
            build(data, targetCellSize, cellSpacing);
        }

        public void build(ShapeArchetype data, Vector2 targetCellSize, Vector2 targetCellSpacing) {
            NullGuard.NotNullOrThrow(data);

            clear();
            cellSize = targetCellSize;
            cellSpacing = targetCellSpacing;
            shapeOffsets = data.Shape.Cells.ToArray();

            foreach (var offset in shapeOffsets) {
                var go = new GameObject(
                    $"Tile_{offset.x}_{offset.y}",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(ItemCellTileView));

                go.transform.SetParent(transform, false);

                var tile = go.GetComponent<ItemCellTileView>();
                tile.bindShapeCell(
                    offset,
                    countCellsBeforeInRow(offset),
                    countCellsInRow(offset.y));
                tile.setupVisual(cellColor);
                tile.setClickHandler(handleClicked);
                tile.setSize(cellSize);

                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(
                    offset.x * (cellSize.x + cellSpacing.x),
                    -offset.y * (cellSize.y + cellSpacing.y)
                );

                itemCellTileViews.Add(tile);
            }

            resizeToFit();
            createCastProgressBarsView();
        }

        public void setOriginInGrid(
            Vector2Int origin,
            Vector2 paramCellSize,
            Vector2 gridOrigin,
            float spacing = 0f) {
            setOriginInGrid(origin, paramCellSize, gridOrigin, new Vector2(spacing, spacing));
        }

        public void setOriginInGrid(
            Vector2Int origin,
            Vector2 paramCellSize,
            Vector2 gridOrigin,
            Vector2 spacing) {
            cellSize = paramCellSize;
            cellSpacing = spacing;

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
            cellColor = c;

            foreach (var itemCellTileView in itemCellTileViews) {
                itemCellTileView.setupVisual(c);
            }
        }

        public void setClickHandler(Action handler) {
            clickHandler = handler;

            for (int i = 0; i < itemCellTileViews.Count; i++) {
                itemCellTileViews[i].setClickHandler(handleClicked);
            }
        }

        public void setFlowPortVisual(
            FlowPortKind flowPortKind,
            string flowPortName,
            string flowPortDescription) {
            if (flowPortKind == FlowPortKind.None) {
                if (flowPortBadgeView != null) {
                    flowPortBadgeView.hide();
                }

                return;
            }

            ensureFlowPortBadgeView();
            flowPortBadgeView.bind(flowPortKind, flowPortName, flowPortDescription);
        }

        internal void setVisualState(InventoryItemVisualState state) {
            for (int i = 0; i < itemCellTileViews.Count; i++) {
                itemCellTileViews[i].setVisualState(cellColor, state);
            }
        }

        public void setCastProgressBars(IReadOnlyList<ItemCastProgressViewState> progressRatios) {
            NullGuard.NotNullOrThrow(progressRatios);
            ensureCastProgressBarsView();
            castProgressBarsView.setProgressBars(progressRatios);
        }

        public void hideCastProgressBars() {
            if (castProgressBarsView != null) {
                castProgressBarsView.hideAll();
            }
        }

        internal Vector2 getCenterInParent() {
            RectTransform rectTransform = (RectTransform)transform;
            return rectTransform.anchoredPosition + new Vector2(
                rectTransform.rect.width * 0.5f,
                -rectTransform.rect.height * 0.5f);
        }

        internal bool tryGetRowCenterInParent(int localRow, out Vector2 point) {
            if (tryGetRowPathInParent(localRow, out Vector2 start, out Vector2 end)) {
                point = Vector2.Lerp(start, end, 0.5f);
                return true;
            }

            point = getCenterInParent();
            return false;
        }

        internal bool tryGetRowPathInParent(int localRow, out Vector2 start, out Vector2 end) {
            if (shapeOffsets == null || shapeOffsets.Length == 0) {
                start = getCenterInParent();
                end = start;
                return false;
            }

            bool foundRow = false;
            int minX = 0;
            int maxX = 0;

            for (int i = 0; i < shapeOffsets.Length; i++) {
                Vector2Int offset = shapeOffsets[i];

                if (offset.y != localRow) {
                    continue;
                }

                if (!foundRow) {
                    minX = offset.x;
                    maxX = offset.x;
                    foundRow = true;
                    continue;
                }

                minX = Mathf.Min(minX, offset.x);
                maxX = Mathf.Max(maxX, offset.x);
            }

            if (!foundRow) {
                start = getCenterInParent();
                end = start;
                return false;
            }

            RectTransform rectTransform = (RectTransform)transform;
            float startX = minX * (cellSize.x + cellSpacing.x) + cellSize.x * 0.5f;
            float endX = maxX * (cellSize.x + cellSpacing.x) + cellSize.x * 0.5f;
            float localY = -localRow * (cellSize.y + cellSpacing.y) - cellSize.y * 0.5f;
            start = rectTransform.anchoredPosition + new Vector2(startX, localY);
            end = rectTransform.anchoredPosition + new Vector2(endX, localY);
            return true;
        }

        private Vector2 calculateAnchoredPosition(
            Vector2Int origin,
            Vector2 paramCellSize,
            Vector2 gridOrigin,
            Vector2 spacing) {
            float x = origin.x * (paramCellSize.x + spacing.x);
            float y = -origin.y * (paramCellSize.y + spacing.y);
            return gridOrigin + new Vector2(x, y);
        }

        private void clear() {
            killMoveTween();

            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            itemCellTileViews.Clear();
            castProgressBarsView = null;
            flowPortBadgeView = null;
        }

        private void handleClicked() {
            clickHandler?.Invoke();
        }

        private void ensureCastProgressBarsView() {
            if (castProgressBarsView == null) {
                createCastProgressBarsView();
            }
        }

        private int countCellsInRow(int localRow) {
            int count = 0;

            for (int i = 0; i < shapeOffsets.Length; i++) {
                if (shapeOffsets[i].y == localRow) {
                    count++;
                }
            }

            return count;
        }

        private int countCellsBeforeInRow(Vector2Int localCell) {
            int count = 0;

            for (int i = 0; i < shapeOffsets.Length; i++) {
                Vector2Int otherCell = shapeOffsets[i];

                if (otherCell.y == localCell.y && otherCell.x < localCell.x) {
                    count++;
                }
            }

            return count;
        }

        private void createCastProgressBarsView() {
            castProgressBarsView = ItemCastProgressBarsView.create(transform);
            castProgressBarsView.bindTiles(itemCellTileViews);
        }

        private void ensureFlowPortBadgeView() {
            if (flowPortBadgeView == null) {
                flowPortBadgeView = FlowPortBadgeView.create(transform);
            }
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

            float width = wCells * cellSize.x + (wCells - 1) * cellSpacing.x;
            float height = hCells * cellSize.y + (hCells - 1) * cellSpacing.y;

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

        private sealed class FlowPortBadgeView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
            private const float BadgeWidth = 38f;
            private const float BadgeHeight = 18f;
            private const float TooltipWidth = 250f;
            private const float TooltipHeight = 86f;

            private RectTransform rectTransform;
            private Image background;
            private TextMeshProUGUI label;
            private GameObject tooltipRoot;
            private TextMeshProUGUI tooltipText;
            private string description;

            internal static FlowPortBadgeView create(Transform parent) {
                GameObject root = new GameObject(
                    "FlowPortBadge",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(FlowPortBadgeView));
                root.transform.SetParent(parent, false);

                FlowPortBadgeView view = root.GetComponent<FlowPortBadgeView>();
                view.initialize();
                return view;
            }

            internal void bind(
                FlowPortKind flowPortKind,
                string flowPortName,
                string flowPortDescription) {
                label.text = string.IsNullOrWhiteSpace(flowPortName)
                    ? flowPortKind.ToString().ToUpperInvariant()
                    : flowPortName;
                description = string.IsNullOrWhiteSpace(flowPortDescription)
                    ? "Flow port."
                    : flowPortDescription;
                background.color = getPortColor(flowPortKind);
                gameObject.SetActive(true);
            }

            internal void hide() {
                hideTooltip();
                gameObject.SetActive(false);
            }

            public void OnPointerEnter(PointerEventData eventData) {
                tooltipText.text = description;
                tooltipRoot.SetActive(true);
            }

            public void OnPointerExit(PointerEventData eventData) {
                hideTooltip();
            }

            private void initialize() {
                rectTransform = (RectTransform)transform;
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(1f, 1f);
                rectTransform.anchoredPosition = new Vector2(-4f, -4f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BadgeWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BadgeHeight);

                background = GetComponent<Image>();
                background.raycastTarget = true;

                label = createText("Label", transform, 10f, FontStyles.Bold);
                RectTransform labelRect = (RectTransform)label.transform;
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;

                createTooltip();
                hide();
            }

            private void createTooltip() {
                tooltipRoot = new GameObject(
                    "FlowPortTooltip",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(CanvasGroup));
                tooltipRoot.transform.SetParent(transform, false);

                RectTransform tooltipRect = (RectTransform)tooltipRoot.transform;
                tooltipRect.anchorMin = new Vector2(1f, 1f);
                tooltipRect.anchorMax = new Vector2(1f, 1f);
                tooltipRect.pivot = new Vector2(1f, 0f);
                tooltipRect.anchoredPosition = new Vector2(0f, BadgeHeight + 6f);
                tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TooltipWidth);
                tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TooltipHeight);

                Image tooltipBackground = tooltipRoot.GetComponent<Image>();
                tooltipBackground.color = new Color(0.03f, 0.05f, 0.07f, 0.95f);
                tooltipBackground.raycastTarget = false;

                CanvasGroup canvasGroup = tooltipRoot.GetComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                tooltipText = createText("Text", tooltipRoot.transform, 11f, FontStyles.Normal);
                RectTransform textRect = (RectTransform)tooltipText.transform;
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(8f, 8f);
                textRect.offsetMax = new Vector2(-8f, -8f);
                tooltipText.alignment = TextAlignmentOptions.TopLeft;
                tooltipText.textWrappingMode = TextWrappingModes.Normal;
                tooltipText.color = new Color(0.9f, 1f, 0.96f, 1f);
                hideTooltip();
            }

            private void hideTooltip() {
                if (tooltipRoot != null) {
                    tooltipRoot.SetActive(false);
                }
            }

            private static TextMeshProUGUI createText(
                string name,
                Transform parent,
                float fontSize,
                FontStyles fontStyle) {
                GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(parent, false);

                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = fontSize;
                text.fontStyle = fontStyle;
                text.color = Color.white;
                text.raycastTarget = false;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                return text;
            }

            private static Color getPortColor(FlowPortKind flowPortKind) {
                return flowPortKind == FlowPortKind.Output
                    ? new Color(0.46f, 0.72f, 1f, 0.95f)
                    : new Color(0.42f, 1f, 0.78f, 0.95f);
            }
        }
    }
}