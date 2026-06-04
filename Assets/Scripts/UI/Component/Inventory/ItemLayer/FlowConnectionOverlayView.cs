using System;
using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal sealed class FlowConnectionOverlayView : MonoBehaviour {
        private const int MAX_VISIBLE_FLOW_SEGMENTS = 180;
        private const float LINE_THICKNESS = 6f;
        private const float DASH_LENGTH = 16f;
        private const float DASH_GAP = 12f;
        private const float DASH_PHASE_STEP = 7f;
        private const float COMET_SIZE = 12f;

        private readonly List<FlowConnectionView> connectionViews = new();
        private readonly Dictionary<RowPathKey, int> flowCountByRow = new();

        internal static FlowConnectionOverlayView create(Transform parent) {
            NullGuard.NotNullOrThrow(parent);

            var go = new GameObject(
                nameof(FlowConnectionOverlayView),
                typeof(RectTransform),
                typeof(FlowConnectionOverlayView));

            go.transform.SetParent(parent, false);
            go.transform.SetAsLastSibling();

            var rectTransform = (RectTransform)go.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return go.GetComponent<FlowConnectionOverlayView>();
        }

        internal void printConnections(
            IReadOnlyList<FlowPathViewState> flowPaths,
            IReadOnlyDictionary<Id<ItemId>, PlacedItemView> itemViews,
            Action<Id<ActiveFlowId>> onFlowClicked) {
            NullGuard.NotNullOrThrow(flowPaths);
            NullGuard.NotNullOrThrow(itemViews);
            transform.SetAsLastSibling();
            rebuildFlowCountByRow(flowPaths);

            int usedConnectionCount = 0;

            for (int flowIndex = 0; flowIndex < flowPaths.Count; flowIndex++) {
                FlowPathViewState flowPath = flowPaths[flowIndex];
                IReadOnlyList<ItemFlowProcessingSlot> processingPath = flowPath.getProcessingPath();
                Color color = FlowVisualPalette.getColor(flowPath.getFlowVisualIndex());
                bool hasPreviousVisibleRow = false;
                Vector2 previousRowEnd = Vector2.zero;

                for (int pathIndex = 0; pathIndex < processingPath.Count; pathIndex++) {
                    if (usedConnectionCount >= MAX_VISIBLE_FLOW_SEGMENTS) {
                        hideUnusedConnections(usedConnectionCount);
                        return;
                    }

                    ItemFlowProcessingSlot slot = processingPath[pathIndex];
                    if (!itemViews.TryGetValue(slot.getItemId(), out PlacedItemView itemView)) {
                        continue;
                    }

                    if (!itemView.tryGetRowPathInParent(slot.getLocalRow(), out Vector2 rowStart, out Vector2 rowEnd)) {
                        continue;
                    }

                    if (hasPreviousVisibleRow) {
                        if (usedConnectionCount >= MAX_VISIBLE_FLOW_SEGMENTS) {
                            hideUnusedConnections(usedConnectionCount);
                            return;
                        }

                        ensureConnectionViewCount(usedConnectionCount + 1);
                        connectionViews[usedConnectionCount].print(
                            previousRowEnd,
                            rowStart,
                            1f,
                            color,
                            false,
                            flowPath.getFlowId(),
                            flowPath.getFlowVisualIndex(),
                            false,
                            onFlowClicked);

                        usedConnectionCount++;
                    }

                    ensureConnectionViewCount(usedConnectionCount + 1);
                    bool isCurrentRow = pathIndex == processingPath.Count - 1;
                    connectionViews[usedConnectionCount].print(
                        rowStart,
                        rowEnd,
                        isCurrentRow ? flowPath.getCurrentProgressRatio() : 1f,
                        color,
                        isCurrentRow,
                        flowPath.getFlowId(),
                        flowPath.getFlowVisualIndex(),
                        isSharedRow(slot),
                        onFlowClicked);

                    usedConnectionCount++;
                    previousRowEnd = rowEnd;
                    hasPreviousVisibleRow = true;
                }
            }

            hideUnusedConnections(usedConnectionCount);
        }

        internal void hideAll() {
            for (int i = 0; i < connectionViews.Count; i++) {
                connectionViews[i].hide();
            }
        }

        private void ensureConnectionViewCount(int requiredCount) {
            while (connectionViews.Count < requiredCount) {
                connectionViews.Add(FlowConnectionView.create(transform, connectionViews.Count));
            }
        }

        private void hideUnusedConnections(int usedConnectionCount) {
            for (int i = usedConnectionCount; i < connectionViews.Count; i++) {
                connectionViews[i].hide();
            }
        }

        private void rebuildFlowCountByRow(IReadOnlyList<FlowPathViewState> flowPaths) {
            flowCountByRow.Clear();

            for (int flowIndex = 0; flowIndex < flowPaths.Count; flowIndex++) {
                IReadOnlyList<ItemFlowProcessingSlot> processingPath = flowPaths[flowIndex].getProcessingPath();

                for (int pathIndex = 0; pathIndex < processingPath.Count; pathIndex++) {
                    ItemFlowProcessingSlot slot = processingPath[pathIndex];
                    var key = new RowPathKey(slot.getItemId(), slot.getLocalRow());

                    flowCountByRow.TryGetValue(key, out int count);
                    flowCountByRow[key] = count + 1;
                }
            }
        }

        private bool isSharedRow(ItemFlowProcessingSlot slot) {
            var key = new RowPathKey(slot.getItemId(), slot.getLocalRow());
            return flowCountByRow.TryGetValue(key, out int count) && count > 1;
        }

        private readonly struct RowPathKey : IEquatable<RowPathKey> {
            private readonly Id<ItemId> itemId;
            private readonly int localRow;

            internal RowPathKey(Id<ItemId> itemId, int localRow) {
                this.itemId = itemId;
                this.localRow = localRow;
            }

            public bool Equals(RowPathKey other) {
                return itemId.Equals(other.itemId) && localRow == other.localRow;
            }

            public override bool Equals(object obj) {
                return obj is RowPathKey other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    return (itemId.GetHashCode() * 397) ^ localRow;
                }
            }
        }

        private sealed class FlowConnectionView {
            private readonly GameObject lineRoot;
            private readonly RectTransform lineRectTransform;
            private readonly FlowConnectionClickTarget clickTarget;
            private readonly List<DashView> dashViews = new();
            private readonly GameObject cometRoot;
            private readonly RectTransform cometRectTransform;
            private readonly Image cometImage;

            private FlowConnectionView(
                GameObject lineRoot,
                RectTransform lineRectTransform,
                FlowConnectionClickTarget clickTarget,
                GameObject cometRoot,
                RectTransform cometRectTransform,
                Image cometImage) {
                this.lineRoot = NullGuard.NotNullOrThrow(lineRoot);
                this.lineRectTransform = NullGuard.NotNullOrThrow(lineRectTransform);
                this.clickTarget = NullGuard.NotNullOrThrow(clickTarget);
                this.cometRoot = NullGuard.NotNullOrThrow(cometRoot);
                this.cometRectTransform = NullGuard.NotNullOrThrow(cometRectTransform);
                this.cometImage = NullGuard.NotNullOrThrow(cometImage);
            }

            internal static FlowConnectionView create(Transform parent, int index) {
                var lineGo = new GameObject(
                    $"FlowConnectionLine_{index}",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(FlowConnectionClickTarget));
                lineGo.transform.SetParent(parent, false);

                var lineRectTransform = (RectTransform)lineGo.transform;
                lineRectTransform.anchorMin = new Vector2(0f, 1f);
                lineRectTransform.anchorMax = new Vector2(0f, 1f);
                lineRectTransform.pivot = new Vector2(0f, 0.5f);

                var lineImage = lineGo.GetComponent<Image>();
                lineImage.color = new Color(1f, 1f, 1f, 0f);
                lineImage.raycastTarget = true;

                var clickTarget = lineGo.GetComponent<FlowConnectionClickTarget>();

                var cometGo = new GameObject(
                    $"FlowConnectionComet_{index}",
                    typeof(RectTransform),
                    typeof(Image));
                cometGo.transform.SetParent(parent, false);

                var cometRectTransform = (RectTransform)cometGo.transform;
                cometRectTransform.anchorMin = new Vector2(0f, 1f);
                cometRectTransform.anchorMax = new Vector2(0f, 1f);
                cometRectTransform.pivot = new Vector2(0.5f, 0.5f);
                cometRectTransform.sizeDelta = new Vector2(COMET_SIZE, COMET_SIZE);
                cometRectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);

                var cometImage = cometGo.GetComponent<Image>();
                cometImage.raycastTarget = false;

                var view = new FlowConnectionView(
                    lineGo,
                    lineRectTransform,
                    clickTarget,
                    cometGo,
                    cometRectTransform,
                    cometImage);
                view.hide();
                return view;
            }

            internal void print(
                Vector2 start,
                Vector2 end,
                float progressRatio,
                Color color,
                bool showComet,
                Id<ActiveFlowId> flowId,
                int flowVisualIndex,
                bool useDashPattern,
                Action<Id<ActiveFlowId>> onFlowClicked) {
                Vector2 delta = end - start;
                float length = delta.magnitude;

                if (length <= 0.01f) {
                    start += new Vector2(-LINE_THICKNESS, 0f);
                    end += new Vector2(LINE_THICKNESS, 0f);
                    delta = end - start;
                    length = delta.magnitude;
                }

                float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                float cometProgress = Mathf.Clamp01(0.18f + progressRatio * 0.72f);

                lineRoot.SetActive(true);
                cometRoot.SetActive(showComet);
                clickTarget.bind(flowId, onFlowClicked);

                lineRectTransform.anchoredPosition = start;
                lineRectTransform.sizeDelta = new Vector2(length, LINE_THICKNESS);
                lineRectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
                if (useDashPattern) {
                    printDashes(length, color, flowVisualIndex);
                }
                else {
                    printSolidLine(length, color);
                }

                if (showComet) {
                    cometRectTransform.anchoredPosition = Vector2.Lerp(start, end, cometProgress);
                    cometImage.color = FlowVisualPalette.withAlpha(color, 0.95f);
                }
            }

            internal void hide() {
                lineRoot.SetActive(false);
                cometRoot.SetActive(false);
                clickTarget.clear();
            }

            private void printDashes(float length, Color color, int flowVisualIndex) {
                float patternLength = DASH_LENGTH + DASH_GAP;
                float phase = (flowVisualIndex % 4) * DASH_PHASE_STEP;
                int requiredDashCount = Mathf.CeilToInt((length + phase) / patternLength);

                ensureDashCount(requiredDashCount);

                for (int i = 0; i < requiredDashCount; i++) {
                    float dashStart = i * patternLength - phase;
                    float visibleStart = Mathf.Max(0f, dashStart);
                    float visibleEnd = Mathf.Min(length, dashStart + DASH_LENGTH);

                    if (visibleEnd <= visibleStart) {
                        dashViews[i].hide();
                        continue;
                    }

                    dashViews[i].print(
                        visibleStart,
                        visibleEnd - visibleStart,
                        FlowVisualPalette.withAlpha(color, 0.72f));
                }

                for (int i = requiredDashCount; i < dashViews.Count; i++) {
                    dashViews[i].hide();
                }
            }

            private void printSolidLine(float length, Color color) {
                ensureDashCount(1);
                dashViews[0].print(
                    0f,
                    length,
                    FlowVisualPalette.withAlpha(color, 0.58f));

                for (int i = 1; i < dashViews.Count; i++) {
                    dashViews[i].hide();
                }
            }

            private void ensureDashCount(int requiredDashCount) {
                while (dashViews.Count < requiredDashCount) {
                    dashViews.Add(DashView.create(lineRoot.transform, dashViews.Count));
                }
            }
        }

        private sealed class DashView {
            private readonly GameObject root;
            private readonly RectTransform rectTransform;
            private readonly Image image;

            private DashView(GameObject root, RectTransform rectTransform, Image image) {
                this.root = NullGuard.NotNullOrThrow(root);
                this.rectTransform = NullGuard.NotNullOrThrow(rectTransform);
                this.image = NullGuard.NotNullOrThrow(image);
            }

            internal static DashView create(Transform parent, int index) {
                var dashGo = new GameObject(
                    $"Dash_{index}",
                    typeof(RectTransform),
                    typeof(Image));
                dashGo.transform.SetParent(parent, false);

                var rectTransform = (RectTransform)dashGo.transform;
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                rectTransform.pivot = new Vector2(0f, 0.5f);

                var image = dashGo.GetComponent<Image>();
                image.raycastTarget = false;

                var dashView = new DashView(dashGo, rectTransform, image);
                dashView.hide();
                return dashView;
            }

            internal void print(float startX, float width, Color color) {
                root.SetActive(true);
                rectTransform.anchoredPosition = new Vector2(startX, 0f);
                rectTransform.sizeDelta = new Vector2(width, LINE_THICKNESS);
                image.color = color;
            }

            internal void hide() {
                root.SetActive(false);
            }
        }

        private sealed class FlowConnectionClickTarget : MonoBehaviour, IPointerClickHandler {
            private Id<ActiveFlowId> flowId;
            private Action<Id<ActiveFlowId>> clickHandler;

            internal void bind(Id<ActiveFlowId> activeFlowId, Action<Id<ActiveFlowId>> handler) {
                flowId = activeFlowId;
                clickHandler = handler;
            }

            internal void clear() {
                flowId = default;
                clickHandler = null;
            }

            public void OnPointerClick(PointerEventData eventData) {
                if (flowId.Value > 0) {
                    clickHandler?.Invoke(flowId);
                }
            }
        }
    }
}