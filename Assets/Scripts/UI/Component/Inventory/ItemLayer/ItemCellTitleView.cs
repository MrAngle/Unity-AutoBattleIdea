using System;
using System.Collections.Generic;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class ItemCellTileView : MonoBehaviour {
        [SerializeField] private Image _image;

        private readonly List<CastProgressLane> castProgressLanes = new();
        private Vector2Int localCell;
        private int rowOrderIndex;
        private int rowCellCount = 1;

        private void Awake() {
            if (!_image) _image = GetComponent<Image>();
            _image.raycastTarget = true; // na razie bez interakcji
        }

        public void setupVisual(Color color) {
            if (!_image) _image = GetComponent<Image>();
            _image.color = color;
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
            Color trackColor,
            Color fillColor) {
            NullGuard.NotNullOrThrow(progressRatios);

            int requiredLaneCount = countProgressForLocalRow(progressRatios);

            if (requiredLaneCount == 0) {
                hideCastProgressLanes();
                return;
            }

            ensureLaneCount(requiredLaneCount, trackColor, fillColor);

            int laneIndex = 0;
            for (int i = 0; i < progressRatios.Count; i++) {
                ItemCastProgressViewState progress = progressRatios[i];

                if (progress.getLocalRow() != localCell.y) {
                    continue;
                }

                CastProgressLane lane = castProgressLanes[laneIndex];
                lane.setActive(true);
                float tileProgress = Mathf.Clamp01(
                    progress.getProgressRatio() * rowCellCount - rowOrderIndex);
                lane.setProgress(tileProgress);
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

        private void ensureLaneCount(int requiredLaneCount, Color trackColor, Color fillColor) {
            while (castProgressLanes.Count < requiredLaneCount) {
                castProgressLanes.Add(createCastProgressLane(castProgressLanes.Count, trackColor, fillColor));
            }

            updateLaneLayout();
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

        private CastProgressLane createCastProgressLane(int index, Color trackColor, Color fillColor) {
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
            fillImage.color = fillColor;
            fillImage.raycastTarget = false;

            var lane = new CastProgressLane(trackGo, trackRectTransform, fillRectTransform);
            lane.setActive(false);
            return lane;
        }

        private void updateLaneLayout() {
            int laneCount = castProgressLanes.Count;

            for (int i = 0; i < laneCount; i++) {
                float minY = (float)i / laneCount;
                float maxY = (float)(i + 1) / laneCount;
                castProgressLanes[i].setVerticalAnchors(minY, maxY);
            }
        }

        private sealed class CastProgressLane {
            private readonly GameObject root;
            private readonly RectTransform trackRectTransform;
            private readonly RectTransform fillRectTransform;

            internal CastProgressLane(
                GameObject root,
                RectTransform trackRectTransform,
                RectTransform fillRectTransform) {
                this.root = NullGuard.NotNullOrThrow(root);
                this.trackRectTransform = NullGuard.NotNullOrThrow(trackRectTransform);
                this.fillRectTransform = NullGuard.NotNullOrThrow(fillRectTransform);
            }

            internal void setActive(bool active) {
                root.SetActive(active);
            }

            internal void setProgress(float progressRatio) {
                fillRectTransform.anchorMax = new Vector2(progressRatio, 1f);
            }

            internal void setVerticalAnchors(float minY, float maxY) {
                trackRectTransform.anchorMin = new Vector2(0f, minY);
                trackRectTransform.anchorMax = new Vector2(1f, maxY);
                trackRectTransform.offsetMin = Vector2.zero;
                trackRectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}