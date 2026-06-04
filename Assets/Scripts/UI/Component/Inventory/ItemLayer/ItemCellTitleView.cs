using System;
using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class ItemCellTileView : MonoBehaviour, IPointerClickHandler {
        private const float CAST_PROGRESS_SMOOTH_SPEED = 9f;

        [SerializeField] private Image _image;

        private readonly List<CastProgressLane> castProgressLanes = new();
        private Outline outline;
        private Action clickHandler;
        private Vector2Int localCell;
        private int rowOrderIndex;
        private int rowCellCount = 1;

        private void Awake() {
            if (!_image) _image = GetComponent<Image>();
            _image.raycastTarget = true;
            ensureOutline();
        }

        public void setupVisual(Color color) {
            if (!_image) _image = GetComponent<Image>();
            setVisualState(color, InventoryItemVisualState.Normal);
        }

        internal void setVisualState(Color baseColor, InventoryItemVisualState state) {
            if (!_image) _image = GetComponent<Image>();

            InventoryItemVisualStyle style = InventoryItemVisualStyle.from(baseColor, state);
            _image.color = style.getFillColor();

            ensureOutline();
            outline.enabled = style.isOutlineEnabled();
            outline.effectColor = style.getOutlineColor();
            outline.effectDistance = style.getOutlineDistance();
        }

        public void setClickHandler(Action handler) {
            clickHandler = handler;
        }

        public void OnPointerClick(PointerEventData eventData) {
            clickHandler?.Invoke();
        }

        public void bindShapeCell(
            Vector2Int localCell,
            int rowOrderIndex,
            int rowCellCount) {
            if (rowOrderIndex < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(rowOrderIndex),
                    rowOrderIndex,
                    "Row order index cannot be negative.");
            }

            if (rowCellCount <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(rowCellCount),
                    rowCellCount,
                    "Row cell count must be positive.");
            }

            this.localCell = localCell;
            this.rowOrderIndex = rowOrderIndex;
            this.rowCellCount = rowCellCount;
        }

        public void setSize(Vector2 size) {
            var rt = (RectTransform)transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        public void setCastProgressLanes(
            IReadOnlyList<ItemCastProgressViewState> progressRatios,
            Color trackColor) {
            NullGuard.NotNullOrThrow(progressRatios);

            int requiredLaneCount = countProgressForLocalRow(progressRatios);

            if (requiredLaneCount == 0) {
                hideCastProgressLanes();
                return;
            }

            ensureLaneCount(requiredLaneCount, trackColor);
            updateLaneLayout(requiredLaneCount);

            int laneIndex = 0;
            for (int i = 0; i < progressRatios.Count; i++) {
                ItemCastProgressViewState progress = progressRatios[i];

                if (progress.getLocalRow() != localCell.y) {
                    continue;
                }

                CastProgressLane lane = castProgressLanes[laneIndex];
                lane.setActive(true);
                lane.setColor(FlowVisualPalette.getColor(progress.getFlowVisualIndex()));
                float tileProgress = Mathf.Clamp01(
                    progress.getProgressRatio() * rowCellCount - rowOrderIndex);
                lane.setTargetProgress(progress.getFlowId(), tileProgress);
                laneIndex++;
            }

            for (int i = laneIndex; i < castProgressLanes.Count; i++) {
                castProgressLanes[i].setActive(false);
            }
        }

        public void hideCastProgressLanes() {
            for (int i = 0; i < castProgressLanes.Count; i++) {
                castProgressLanes[i].setActive(false);
            }
        }

        internal void smoothCastProgressLanes(float deltaTime) {
            for (int i = 0; i < castProgressLanes.Count; i++) {
                castProgressLanes[i].smoothProgress(deltaTime);
            }
        }

        internal int getLocalRow() {
            return localCell.y;
        }

        private void ensureOutline() {
            if (outline == null) {
                outline = GetComponent<Outline>();

                if (outline == null) {
                    outline = gameObject.AddComponent<Outline>();
                }
            }
        }

        private void ensureLaneCount(int requiredLaneCount, Color trackColor) {
            while (castProgressLanes.Count < requiredLaneCount) {
                castProgressLanes.Add(createCastProgressLane(castProgressLanes.Count, trackColor));
            }
        }

        private int countProgressForLocalRow(IReadOnlyList<ItemCastProgressViewState> progressRatios) {
            int count = 0;

            for (int i = 0; i < progressRatios.Count; i++) {
                if (progressRatios[i].getLocalRow() == localCell.y) {
                    count++;
                }
            }

            return count;
        }

        private CastProgressLane createCastProgressLane(int index, Color trackColor) {
            var trackGo = new GameObject(
                $"CastProgressLane_{index}",
                typeof(RectTransform),
                typeof(Image));

            trackGo.transform.SetParent(transform, false);

            var trackRectTransform = (RectTransform)trackGo.transform;
            trackRectTransform.anchorMin = Vector2.zero;
            trackRectTransform.anchorMax = Vector2.one;
            trackRectTransform.offsetMin = Vector2.zero;
            trackRectTransform.offsetMax = Vector2.zero;

            var trackImage = trackGo.GetComponent<Image>();
            trackImage.color = trackColor;
            trackImage.raycastTarget = false;

            var fillGo = new GameObject(
                "Fill",
                typeof(RectTransform),
                typeof(Image));

            fillGo.transform.SetParent(trackGo.transform, false);

            var fillRectTransform = (RectTransform)fillGo.transform;
            fillRectTransform.anchorMin = new Vector2(0f, 0f);
            fillRectTransform.anchorMax = new Vector2(0f, 1f);
            fillRectTransform.offsetMin = Vector2.zero;
            fillRectTransform.offsetMax = Vector2.zero;

            var fillImage = fillGo.GetComponent<Image>();
            fillImage.raycastTarget = false;

            var cometGo = new GameObject(
                "Comet",
                typeof(RectTransform),
                typeof(Image));

            cometGo.transform.SetParent(trackGo.transform, false);

            var cometRectTransform = (RectTransform)cometGo.transform;
            cometRectTransform.anchorMin = new Vector2(0f, 0.5f);
            cometRectTransform.anchorMax = new Vector2(0f, 0.5f);
            cometRectTransform.pivot = new Vector2(0.5f, 0.5f);
            cometRectTransform.sizeDelta = new Vector2(7f, 7f);

            var cometImage = cometGo.GetComponent<Image>();
            cometImage.raycastTarget = false;

            var lane = new CastProgressLane(
                trackGo,
                trackRectTransform,
                fillRectTransform,
                fillImage,
                cometRectTransform,
                cometImage);
            lane.setActive(false);
            return lane;
        }

        private void updateLaneLayout(int visibleLaneCount) {
            for (int i = 0; i < visibleLaneCount; i++) {
                float minY = (float)i / visibleLaneCount;
                float maxY = (float)(i + 1) / visibleLaneCount;
                castProgressLanes[i].setVerticalAnchors(minY, maxY);
            }
        }

        private sealed class CastProgressLane {
            private readonly GameObject root;
            private readonly RectTransform trackRectTransform;
            private readonly RectTransform fillRectTransform;
            private readonly Image fillImage;
            private readonly RectTransform cometRectTransform;
            private readonly Image cometImage;
            private Id<ActiveFlowId> flowId;
            private float visibleProgressRatio;
            private float targetProgressRatio;
            private bool hasProgress;
            private bool active;

            internal CastProgressLane(
                GameObject root,
                RectTransform trackRectTransform,
                RectTransform fillRectTransform,
                Image fillImage,
                RectTransform cometRectTransform,
                Image cometImage) {
                this.root = NullGuard.NotNullOrThrow(root);
                this.trackRectTransform = NullGuard.NotNullOrThrow(trackRectTransform);
                this.fillRectTransform = NullGuard.NotNullOrThrow(fillRectTransform);
                this.fillImage = NullGuard.NotNullOrThrow(fillImage);
                this.cometRectTransform = NullGuard.NotNullOrThrow(cometRectTransform);
                this.cometImage = NullGuard.NotNullOrThrow(cometImage);
            }

            internal void setActive(bool active) {
                this.active = active;
                root.SetActive(active);

                if (!active) {
                    flowId = default;
                    hasProgress = false;
                    visibleProgressRatio = 0f;
                    targetProgressRatio = 0f;
                    applyProgress(0f);
                }
            }

            internal void setTargetProgress(Id<ActiveFlowId> newFlowId, float progressRatio) {
                float clampedProgressRatio = Mathf.Clamp01(progressRatio);

                if (!hasProgress || flowId != newFlowId || clampedProgressRatio < visibleProgressRatio) {
                    flowId = newFlowId;
                    visibleProgressRatio = clampedProgressRatio;
                    targetProgressRatio = clampedProgressRatio;
                    hasProgress = true;
                    applyProgress(visibleProgressRatio);
                    return;
                }

                targetProgressRatio = clampedProgressRatio;
            }

            internal void smoothProgress(float deltaTime) {
                if (!active || !hasProgress) {
                    return;
                }

                if (Mathf.Approximately(visibleProgressRatio, targetProgressRatio)) {
                    return;
                }

                visibleProgressRatio = Mathf.MoveTowards(
                    visibleProgressRatio,
                    targetProgressRatio,
                    CAST_PROGRESS_SMOOTH_SPEED * deltaTime);
                applyProgress(visibleProgressRatio);
            }

            private void applyProgress(float progressRatio) {
                float clampedProgressRatio = Mathf.Clamp01(progressRatio);
                fillRectTransform.anchorMax = new Vector2(clampedProgressRatio, 1f);
                cometRectTransform.anchorMin = new Vector2(clampedProgressRatio, 0.5f);
                cometRectTransform.anchorMax = new Vector2(clampedProgressRatio, 0.5f);
            }

            internal void setVerticalAnchors(float minY, float maxY) {
                trackRectTransform.anchorMin = new Vector2(0f, minY);
                trackRectTransform.anchorMax = new Vector2(1f, maxY);
                trackRectTransform.offsetMin = Vector2.zero;
                trackRectTransform.offsetMax = Vector2.zero;
            }

            internal void setColor(Color color) {
                fillImage.color = color;
                cometImage.color = FlowVisualPalette.withAlpha(color, 1f);
            }
        }
    }
}