using System.Collections.Generic;
using DG.Tweening;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal sealed class InventoryActionBeamOverlayView : MonoBehaviour {
        private const float BeamThickness = 5f;
        private const float BeamDuration = 0.55f;
        private const float BeamHeadSize = 12f;

        private readonly List<ActionBeamView> beamViews = new();

        internal static InventoryActionBeamOverlayView create(Transform parent) {
            NullGuard.NotNullOrThrow(parent);

            var go = new GameObject(
                nameof(InventoryActionBeamOverlayView),
                typeof(RectTransform),
                typeof(InventoryActionBeamOverlayView));

            go.transform.SetParent(parent, false);
            go.transform.SetAsLastSibling();

            var rectTransform = (RectTransform)go.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return go.GetComponent<InventoryActionBeamOverlayView>();
        }

        internal void showBeam(Vector2 start, Vector2 end, Color color) {
            transform.SetAsLastSibling();
            ActionBeamView beamView = getAvailableBeamView();
            beamView.show(start, end, color);
        }

        private ActionBeamView getAvailableBeamView() {
            for (int i = 0; i < beamViews.Count; i++) {
                if (!beamViews[i].isActive()) {
                    return beamViews[i];
                }
            }

            ActionBeamView beamView = ActionBeamView.create(transform, beamViews.Count);
            beamViews.Add(beamView);
            return beamView;
        }

        private sealed class ActionBeamView {
            private readonly GameObject root;
            private readonly RectTransform lineRectTransform;
            private readonly Image lineImage;
            private readonly RectTransform headRectTransform;
            private readonly Image headImage;
            private Sequence sequence;

            private ActionBeamView(
                GameObject root,
                RectTransform lineRectTransform,
                Image lineImage,
                RectTransform headRectTransform,
                Image headImage) {
                this.root = NullGuard.NotNullOrThrow(root);
                this.lineRectTransform = NullGuard.NotNullOrThrow(lineRectTransform);
                this.lineImage = NullGuard.NotNullOrThrow(lineImage);
                this.headRectTransform = NullGuard.NotNullOrThrow(headRectTransform);
                this.headImage = NullGuard.NotNullOrThrow(headImage);
            }

            internal static ActionBeamView create(Transform parent, int index) {
                GameObject root = new GameObject(
                    $"ActionBeam_{index}",
                    typeof(RectTransform));
                root.transform.SetParent(parent, false);

                var rootRectTransform = (RectTransform)root.transform;
                rootRectTransform.anchorMin = new Vector2(0f, 1f);
                rootRectTransform.anchorMax = new Vector2(0f, 1f);
                rootRectTransform.pivot = new Vector2(0f, 0.5f);

                GameObject line = new GameObject(
                    "Line",
                    typeof(RectTransform),
                    typeof(Image));
                line.transform.SetParent(root.transform, false);

                var lineRectTransform = (RectTransform)line.transform;
                lineRectTransform.anchorMin = new Vector2(0f, 0.5f);
                lineRectTransform.anchorMax = new Vector2(0f, 0.5f);
                lineRectTransform.pivot = new Vector2(0f, 0.5f);

                Image lineImage = line.GetComponent<Image>();
                lineImage.raycastTarget = false;

                GameObject head = new GameObject(
                    "Head",
                    typeof(RectTransform),
                    typeof(Image));
                head.transform.SetParent(root.transform, false);

                var headRectTransform = (RectTransform)head.transform;
                headRectTransform.anchorMin = new Vector2(0f, 0.5f);
                headRectTransform.anchorMax = new Vector2(0f, 0.5f);
                headRectTransform.pivot = new Vector2(0.5f, 0.5f);
                headRectTransform.sizeDelta = new Vector2(BeamHeadSize, BeamHeadSize);
                headRectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);

                Image headImage = head.GetComponent<Image>();
                headImage.raycastTarget = false;

                var view = new ActionBeamView(
                    root,
                    lineRectTransform,
                    lineImage,
                    headRectTransform,
                    headImage);
                view.hide();
                return view;
            }

            internal bool isActive() {
                return root.activeSelf;
            }

            internal void show(Vector2 start, Vector2 end, Color color) {
                killSequence();

                Vector2 delta = end - start;
                float length = Mathf.Max(BeamThickness, delta.magnitude);
                float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                Color visibleColor = new Color(color.r, color.g, color.b, 0.9f);

                root.SetActive(true);
                RectTransform rootRectTransform = (RectTransform)root.transform;
                rootRectTransform.anchoredPosition = start;
                rootRectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

                lineRectTransform.anchoredPosition = Vector2.zero;
                lineRectTransform.sizeDelta = new Vector2(length, BeamThickness);
                lineImage.color = visibleColor;

                headRectTransform.anchoredPosition = new Vector2(length, 0f);
                headImage.color = visibleColor;

                sequence = DOTween.Sequence();
                sequence.Append(lineImage.DOFade(0f, BeamDuration));
                sequence.Join(headImage.DOFade(0f, BeamDuration));
                sequence.OnComplete(hide);
            }

            private void hide() {
                root.SetActive(false);
            }

            private void killSequence() {
                if (sequence != null && sequence.IsActive()) {
                    sequence.Kill();
                    sequence = null;
                }
            }
        }
    }
}