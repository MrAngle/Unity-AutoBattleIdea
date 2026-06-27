using System;
using DG.Tweening;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Shared.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal sealed class DefenseLayerOverlayView : MonoBehaviour {
        internal const float RootPadding = 4f;
        internal const float LayerSpacing = 6f;
        internal const float LayerRowHeight = 34f;
        internal const float GuardBottomOffset = LayerRowHeight + LayerSpacing + RootPadding;

        private DefenseLayerRowView stabilityRow;
        private DefenseLayerRowView hpRow;
        private RectTransform rectTransform;

        internal static DefenseLayerOverlayView create(Transform parent) {
            NullGuard.NotNullOrThrow(parent);

            GameObject root = new GameObject(
                "DefenseLayerOverlay",
                typeof(RectTransform),
                typeof(DefenseLayerOverlayView));
            root.transform.SetParent(parent, false);

            DefenseLayerOverlayView view = root.GetComponent<DefenseLayerOverlayView>();
            view.initialize();
            return view;
        }

        internal void printDefenseLayers(
            long currentStability,
            long baselineStability,
            long currentHp,
            long maxHp,
            ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            gameObject.SetActive(true);
            configureRoot(inventoryGridInfo);

            stabilityRow.bind(
                "1 Stabilnosc",
                GuardPowerLabelFormatter.formatNonNegativeOrSigned(currentStability) + " / "
                + GuardPowerLabelFormatter.format(baselineStability),
                "~" + calculateStabilityReductionPercent(currentStability, baselineStability) + "%",
                calculateStabilityFill(currentStability, baselineStability),
                currentStability >= baselineStability
                    ? new Color(0.36f, 0.86f, 1f, 0.85f)
                    : new Color(1f, 0.72f, 0.28f, 0.85f),
                new Color(0.05f, 0.12f, 0.16f, 0.9f));

            hpRow.bind(
                "3 HP",
                GuardPowerLabelFormatter.formatNonNegativeOrSigned(currentHp) + " / "
                                                                              + GuardPowerLabelFormatter.format(
                                                                                  Math.Max(0, maxHp)),
                calculateHpPercent(currentHp, maxHp) + "%",
                calculateHpFill(currentHp, maxHp),
                new Color(1f, 0.28f, 0.25f, 0.84f),
                new Color(0.14f, 0.05f, 0.05f, 0.9f));
        }

        internal bool tryGetStabilityCenterInParent(out Vector2 center) {
            if (stabilityRow == null || !gameObject.activeSelf) {
                center = default;
                return false;
            }

            center = rectTransform.anchoredPosition + stabilityRow.getCenterInOverlay();
            return true;
        }

        internal void showStabilityGeneratedVisual(long stabilityPower) {
            stabilityRow.playVisual(
                new Color(0.36f, 0.86f, 1f, 1f),
                "S +" + GuardPowerLabelFormatter.format(stabilityPower));
        }

        internal void showStabilityAbsorbedVisual(long reducedDamage, long stabilityStrain, long remainingDamage) {
            string label = "S -" + GuardPowerLabelFormatter.format(stabilityStrain)
                                 + " / " + GuardPowerLabelFormatter.format(remainingDamage);
            stabilityRow.playVisual(new Color(1f, 0.72f, 0.28f, 1f), label);
        }

        internal void showHpChangedVisual(long delta) {
            Color color = delta < 0
                ? new Color(1f, 0.28f, 0.25f, 1f)
                : new Color(0.34f, 1f, 0.55f, 1f);
            string prefix = delta < 0 ? "" : "+";
            hpRow.playVisual(color, "HP " + prefix + GuardPowerLabelFormatter.formatNonNegativeOrSigned(delta));
        }

        private void initialize() {
            rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);

            stabilityRow = DefenseLayerRowView.create("StabilityRow", transform);
            hpRow = DefenseLayerRowView.create("HpRow", transform);
            gameObject.SetActive(false);
        }

        private void configureRoot(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float width = calculateInventoryWidth(inventoryGridInfo);
            float totalHeight = LayerRowHeight
                                + LayerSpacing
                                + PreparedGuardOverlayView.ReservedHeight
                                + LayerSpacing
                                + LayerRowHeight;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            rectTransform.anchoredPosition = new Vector2(RootPadding, totalHeight + RootPadding);

            stabilityRow.setSize(width, LayerRowHeight);
            stabilityRow.setAnchoredPosition(Vector2.zero);

            hpRow.setSize(width, LayerRowHeight);
            hpRow.setAnchoredPosition(new Vector2(
                0f,
                -(LayerRowHeight + LayerSpacing + PreparedGuardOverlayView.ReservedHeight + LayerSpacing)));
        }

        private static float calculateInventoryWidth(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            int widthCells = Mathf.Max(1, inventoryGridInfo.WidthCellsNumber);
            return widthCells * inventoryGridInfo.CellSize.x
                   + Mathf.Max(0, widthCells - 1) * inventoryGridInfo.Spacing.x;
        }

        private static int calculateStabilityReductionPercent(long stability, long baselineStability) {
            long sampleDamage = Math.Max(1, baselineStability);
            long reduced = StabilityMitigationCalculator.calculateReducedDamage(
                stability,
                baselineStability,
                sampleDamage);
            return Mathf.Clamp(Mathf.RoundToInt((float)reduced * 100f / sampleDamage), 0, 999);
        }

        private static float calculateStabilityFill(long stability, long baselineStability) {
            if (baselineStability <= 0) {
                return 0f;
            }

            float ratio = (float)stability / (baselineStability * 2f);
            return Mathf.Clamp01(ratio);
        }

        private static int calculateHpPercent(long currentHp, long maxHp) {
            if (maxHp <= 0) {
                return 0;
            }

            return Mathf.Clamp(Mathf.RoundToInt((float)currentHp * 100f / maxHp), 0, 999);
        }

        private static float calculateHpFill(long currentHp, long maxHp) {
            if (maxHp <= 0) {
                return 0f;
            }

            return Mathf.Clamp01((float)currentHp / maxHp);
        }

        private sealed class DefenseLayerRowView : MonoBehaviour {
            private RectTransform rectTransform;
            private Image backgroundImage;
            private Image fillImage;
            private TextMeshProUGUI titleText;
            private TextMeshProUGUI valueText;
            private TextMeshProUGUI detailText;
            private Sequence visualSequence;

            internal static DefenseLayerRowView create(string name, Transform parent) {
                GameObject root = new GameObject(
                    name,
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(Outline),
                    typeof(DefenseLayerRowView));
                root.transform.SetParent(parent, false);

                DefenseLayerRowView rowView = root.GetComponent<DefenseLayerRowView>();
                rowView.initialize();
                return rowView;
            }

            internal void bind(
                string title,
                string value,
                string detail,
                float fillRatio,
                Color fillColor,
                Color backgroundColor) {
                titleText.text = title;
                valueText.text = value;
                detailText.text = detail;
                backgroundImage.color = backgroundColor;
                fillImage.color = fillColor;
                fillImage.fillAmount = Mathf.Clamp01(fillRatio);
                gameObject.SetActive(true);
            }

            internal void setSize(float width, float height) {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            internal void setAnchoredPosition(Vector2 anchoredPosition) {
                rectTransform.anchoredPosition = anchoredPosition;
            }

            internal Vector2 getCenterInOverlay() {
                return rectTransform.anchoredPosition + new Vector2(
                    rectTransform.rect.width * 0.5f,
                    -rectTransform.rect.height * 0.5f);
            }

            internal void playVisual(Color color, string label) {
                playPunch(color);
                PopupManager.Instance?.Show(
                    this,
                    label,
                    color,
                    moveY: 30f,
                    duration: 0.75f);
            }

            private void initialize() {
                rectTransform = (RectTransform)transform;
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);

                backgroundImage = GetComponent<Image>();
                backgroundImage.raycastTarget = false;

                Outline outline = GetComponent<Outline>();
                outline.effectColor = new Color(1f, 1f, 1f, 0.16f);
                outline.effectDistance = new Vector2(1f, -1f);

                fillImage = createFillImage(transform);
                titleText = createText("Title", transform, 12f, FontStyles.Bold, TextAlignmentOptions.Left);
                valueText = createText("Value", transform, 13f, FontStyles.Bold, TextAlignmentOptions.Center);
                detailText = createText("Detail", transform, 11f, FontStyles.Bold, TextAlignmentOptions.Right);

                RectTransform titleRect = (RectTransform)titleText.transform;
                titleRect.anchorMin = new Vector2(0f, 0f);
                titleRect.anchorMax = new Vector2(0.34f, 1f);
                titleRect.offsetMin = new Vector2(8f, 0f);
                titleRect.offsetMax = new Vector2(-4f, 0f);

                RectTransform valueRect = (RectTransform)valueText.transform;
                valueRect.anchorMin = new Vector2(0.34f, 0f);
                valueRect.anchorMax = new Vector2(0.74f, 1f);
                valueRect.offsetMin = new Vector2(2f, 0f);
                valueRect.offsetMax = new Vector2(-2f, 0f);

                RectTransform detailRect = (RectTransform)detailText.transform;
                detailRect.anchorMin = new Vector2(0.74f, 0f);
                detailRect.anchorMax = Vector2.one;
                detailRect.offsetMin = new Vector2(4f, 0f);
                detailRect.offsetMax = new Vector2(-8f, 0f);
            }

            private void playPunch(Color color) {
                if (visualSequence != null && visualSequence.IsActive()) {
                    visualSequence.Kill();
                }

                Color originalColor = backgroundImage.color;
                visualSequence = DOTween.Sequence();
                visualSequence.Append(backgroundImage.DOColor(color, 0.08f));
                visualSequence.Join(rectTransform.DOPunchScale(new Vector3(0.04f, 0.12f, 0f), 0.22f, 6, 0.6f));
                visualSequence.Append(backgroundImage.DOColor(originalColor, 0.18f));
            }

            private static Image createFillImage(Transform parent) {
                GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                fill.transform.SetParent(parent, false);

                RectTransform fillRect = (RectTransform)fill.transform;
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = new Vector2(2f, 2f);
                fillRect.offsetMax = new Vector2(-2f, -2f);

                Image image = fill.GetComponent<Image>();
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.raycastTarget = false;
                return image;
            }

            private static TextMeshProUGUI createText(
                string name,
                Transform parent,
                float fontSize,
                FontStyles fontStyle,
                TextAlignmentOptions alignment) {
                GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(parent, false);

                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
                text.alignment = alignment;
                text.fontSize = fontSize;
                text.fontStyle = fontStyle;
                text.color = Color.white;
                text.raycastTarget = false;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                return text;
            }
        }
    }
}