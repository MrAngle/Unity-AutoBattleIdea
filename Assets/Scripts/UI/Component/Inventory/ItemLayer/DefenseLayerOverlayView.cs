using System;
using System.Collections.Generic;
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
        internal const float StabilityLayerHeight = 66f;
        internal const float HpLayerHeight = 46f;
        internal const float LayerRowHeight = HpLayerHeight;
        internal const float GuardBottomOffset = StabilityLayerHeight + LayerSpacing + RootPadding;
        internal const int VisibleDamagePacketCapacity = 5;
        internal const float DamagePacketValueWidth = 34f;
        internal const float DamagePacketValueGap = 10f;
        internal const float PacketLaneHorizontalPadding = 14f;

        internal const float PacketLaneWidth = VisibleDamagePacketCapacity * DamagePacketValueWidth
                                               + (VisibleDamagePacketCapacity - 1) * DamagePacketValueGap
                                               + PacketLaneHorizontalPadding * 2f;

        internal const float LayerContentOffsetX = PacketLaneWidth + LayerSpacing;
        private const float MinimumDefenseOverlayWidth = LayerContentOffsetX + 420f;

        private static class DefenseLayerPalette {
            internal static readonly Color MainBackground = fromHex("#100C17");
            internal static readonly Color Panel = fromHex("#1C1426", 0.94f);
            internal static readonly Color PanelRaised = fromHex("#291C38", 0.86f);
            internal static readonly Color Border = fromHex("#554165", 0.82f);
            internal static readonly Color OldGold = fromHex("#C49A52", 0.9f);
            internal static readonly Color MainText = fromHex("#E9E2EE");
            internal static readonly Color SecondaryText = fromHex("#B1A5B7");
            internal static readonly Color DefenseBlue = fromHex("#4F78A8");
            internal static readonly Color HealingGreen = fromHex("#639775");
            internal static readonly Color WarningAmber = fromHex("#D18B45");
            internal static readonly Color PowerRed = fromHex("#C6535D");
            internal static readonly Color CriticalRed = fromHex("#A93644");
            internal static readonly Color CriticalBlackRed = fromHex("#220008");
            internal static readonly Color HpFill = fromHex("#C6535D", 0.9f);

            private static Color fromHex(string hex, float alpha = 1f) {
                ColorUtility.TryParseHtmlString(hex, out Color color);
                color.a = alpha;
                return color;
            }
        }

        private DefenseLayerRowView stabilityRow;
        private DefenseLayerRowView hpRow;
        private DamagePacketPathView damagePacketPathView;
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
            damagePacketPathView.showPanel();

            int damageReductionPercent = calculateStabilityDamageReductionPercent(
                currentStability,
                baselineStability);
            Color damageReductionColor = calculateDamageReductionColor(damageReductionPercent);

            stabilityRow.bindStability(
                "Stability",
                GuardPowerLabelFormatter.formatNonNegativeOrSigned(currentStability) + " / "
                + GuardPowerLabelFormatter.format(baselineStability),
                "Damage Reduction: " + damageReductionPercent + "%",
                buildStabilityDecayLabel(currentStability, baselineStability),
                calculateStabilityFill(currentStability, baselineStability),
                withAlpha(damageReductionColor, 0.34f),
                damageReductionColor,
                DefenseLayerPalette.Panel);

            hpRow.bindHp(
                "HP",
                GuardPowerLabelFormatter.formatNonNegativeOrSigned(currentHp) + " / "
                                                                              + GuardPowerLabelFormatter.format(
                                                                                  Math.Max(0, maxHp)),
                calculateHpFill(currentHp, maxHp),
                DefenseLayerPalette.HpFill,
                DefenseLayerPalette.Panel);
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

        internal void showDamagePacketLayer(
            long packetId,
            int layerIndex,
            long damageValue,
            bool completesPacket) {
            if (!gameObject.activeSelf || damagePacketPathView == null) {
                return;
            }

            damagePacketPathView.showLayer(packetId, layerIndex, Math.Max(0, damageValue), completesPacket);
        }

        private void initialize() {
            rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);

            stabilityRow = DefenseLayerRowView.create("StabilityRow", transform);
            hpRow = DefenseLayerRowView.create("HpRow", transform);
            damagePacketPathView = DamagePacketPathView.create("DamagePacketPath", transform);
            damagePacketPathView.transform.SetAsLastSibling();
            gameObject.SetActive(false);
        }

        private void configureRoot(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float width = calculateOverlayWidth(inventoryGridInfo);
            float layerContentWidth = calculateLayerContentWidth(width);
            float totalHeight = StabilityLayerHeight
                                + LayerSpacing
                                + PreparedGuardOverlayView.ReservedHeight
                                + LayerSpacing
                                + HpLayerHeight;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            rectTransform.anchoredPosition = new Vector2(RootPadding, totalHeight + RootPadding);

            stabilityRow.setSize(layerContentWidth, StabilityLayerHeight);
            stabilityRow.setAnchoredPosition(new Vector2(LayerContentOffsetX, 0f));

            hpRow.setSize(layerContentWidth, HpLayerHeight);
            hpRow.setAnchoredPosition(new Vector2(
                LayerContentOffsetX,
                -(StabilityLayerHeight + LayerSpacing + PreparedGuardOverlayView.ReservedHeight + LayerSpacing)));

            damagePacketPathView.setSize(PacketLaneWidth, totalHeight);
            damagePacketPathView.setAnchoredPosition(Vector2.zero);
            damagePacketPathView.transform.SetAsLastSibling();
        }

        private float calculateOverlayWidth(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float inventoryWidth = calculateInventoryWidth(inventoryGridInfo);
            float parentWidth = 0f;
            if (transform.parent is RectTransform parentRectTransform) {
                parentWidth = parentRectTransform.rect.width;
            }

            return Mathf.Max(MinimumDefenseOverlayWidth, inventoryWidth, parentWidth);
        }

        private static float calculateInventoryWidth(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            int widthCells = Mathf.Max(1, inventoryGridInfo.WidthCellsNumber);
            return widthCells * inventoryGridInfo.CellSize.x
                   + Mathf.Max(0, widthCells - 1) * inventoryGridInfo.Spacing.x;
        }

        internal static float calculateLayerContentWidth(float totalWidth) {
            return Mathf.Max(80f, totalWidth - LayerContentOffsetX);
        }

        private static int calculateStabilityDamageReductionPercent(long stability, long baselineStability) {
            long sampleDamage = Math.Max(1, baselineStability);
            long damageAfterStability = StabilityMitigationCalculator.calculateDamageAfterStability(
                stability,
                baselineStability,
                sampleDamage);
            double reductionPercent = (sampleDamage - damageAfterStability) * 100.0 / sampleDamage;
            return Mathf.Clamp(Mathf.RoundToInt((float)reductionPercent), -999, 999);
        }

        private static string buildStabilityDecayLabel(long currentStability, long baselineStability) {
            long decayThreshold = calculateStabilityDecayThreshold(baselineStability);
            long decayPerTick = baselineStability > 0
                ? StabilityMitigationCalculator.calculateOverBaselineDecay(currentStability, baselineStability)
                : 0;
            return "DecayAt: " + GuardPowerLabelFormatter.format(decayThreshold) + "\n"
                   + "Decay/tick: " + GuardPowerLabelFormatter.format(decayPerTick);
        }

        private static long calculateStabilityDecayThreshold(long baselineStability) {
            if (baselineStability <= 0) {
                return 0;
            }

            if (baselineStability > long.MaxValue
                / StabilityMitigationCalculator.OverBaselineDecayThresholdNumerator) {
                return long.MaxValue;
            }

            long multiplied = baselineStability
                              * StabilityMitigationCalculator.OverBaselineDecayThresholdNumerator;
            long divisor = StabilityMitigationCalculator.OverBaselineDecayThresholdDenominator;
            return multiplied / divisor + (multiplied % divisor == 0 ? 0 : 1);
        }

        private static Color calculateDamageReductionColor(int damageReductionPercent) {
            if (damageReductionPercent < 0) {
                return lerp(
                    DefenseLayerPalette.CriticalBlackRed,
                    DefenseLayerPalette.CriticalRed,
                    Mathf.Clamp01((damageReductionPercent + 100f) / 100f));
            }

            if (damageReductionPercent < 15) {
                return lerp(DefenseLayerPalette.CriticalRed, DefenseLayerPalette.PowerRed,
                    damageReductionPercent / 15f);
            }

            if (damageReductionPercent < 30) {
                return lerp(DefenseLayerPalette.PowerRed, DefenseLayerPalette.WarningAmber,
                    (damageReductionPercent - 15f) / 15f);
            }

            if (damageReductionPercent < 45) {
                return lerp(DefenseLayerPalette.WarningAmber, DefenseLayerPalette.DefenseBlue,
                    (damageReductionPercent - 30f) / 15f);
            }

            if (damageReductionPercent < 55) {
                return DefenseLayerPalette.DefenseBlue;
            }

            if (damageReductionPercent < 65) {
                return lerp(DefenseLayerPalette.DefenseBlue, DefenseLayerPalette.HealingGreen,
                    (damageReductionPercent - 55f) / 10f);
            }

            return DefenseLayerPalette.HealingGreen;
        }

        private static Color withAlpha(Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        private static Color lerp(Color from, Color to, float ratio) {
            return Color.Lerp(from, to, Mathf.Clamp01(ratio));
        }

        private static float calculateStabilityFill(long stability, long baselineStability) {
            if (baselineStability <= 0) {
                return 0f;
            }

            float ratio = (float)stability / baselineStability;
            return Mathf.Clamp01(ratio);
        }

        private static float calculateHpFill(long currentHp, long maxHp) {
            if (maxHp <= 0) {
                return 0f;
            }

            return Mathf.Clamp01((float)currentHp / maxHp);
        }

        private sealed class DefenseLayerRowView : MonoBehaviour {
            private RectTransform rectTransform;
            private RectTransform fillRectTransform;
            private RectTransform fillFrameRectTransform;
            private RectTransform titleRectTransform;
            private RectTransform valueRectTransform;
            private RectTransform detailRectTransform;
            private RectTransform extraRectTransform;
            private Vector2 fillAreaAnchorMin = Vector2.zero;
            private Vector2 fillAreaAnchorMax = Vector2.one;
            private Vector2 fillAreaOffsetMin = new Vector2(3f, 3f);
            private Vector2 fillAreaOffsetMax = new Vector2(-3f, -3f);
            private Image backgroundImage;
            private Image fillFrameImage;
            private Image fillImage;
            private TextMeshProUGUI titleText;
            private TextMeshProUGUI valueText;
            private TextMeshProUGUI detailText;
            private TextMeshProUGUI extraText;
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

            internal void bindStability(
                string title,
                string value,
                string detail,
                string extra,
                float fillRatio,
                Color fillColor,
                Color detailColor,
                Color backgroundColor) {
                bindBase(title, value, detail, fillRatio, fillColor, backgroundColor);
                extraText.text = extra;
                configureFillArea(Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f), false);
                setFillRatio(fillRatio);

                titleText.fontSize = 17f;
                valueText.fontSize = 21f;
                detailText.fontSize = 15f;
                extraText.fontSize = 13f;
                titleText.alignment = TextAlignmentOptions.Left;
                valueText.alignment = TextAlignmentOptions.Center;
                detailText.alignment = TextAlignmentOptions.Center;
                extraText.alignment = TextAlignmentOptions.Right;
                titleText.color = DefenseLayerPalette.MainText;
                valueText.color = DefenseLayerPalette.MainText;
                detailText.color = detailColor;
                extraText.color = DefenseLayerPalette.SecondaryText;
                detailText.gameObject.SetActive(true);
                extraText.gameObject.SetActive(true);

                setRect(
                    titleRectTransform,
                    Vector2.zero,
                    new Vector2(0.22f, 1f),
                    new Vector2(12f, 0f),
                    new Vector2(-4f, 0f));
                setRect(
                    valueRectTransform,
                    new Vector2(0.22f, 0f),
                    new Vector2(0.42f, 1f),
                    new Vector2(4f, 0f),
                    new Vector2(-4f, 0f));
                setRect(
                    detailRectTransform,
                    new Vector2(0.42f, 0f),
                    new Vector2(0.69f, 1f),
                    new Vector2(4f, 0f),
                    new Vector2(-4f, 0f));
                setRect(
                    extraRectTransform,
                    new Vector2(0.7f, 0f),
                    Vector2.one,
                    new Vector2(4f, 3f),
                    new Vector2(-12f, -3f));
            }

            internal void bindHp(
                string title,
                string value,
                float fillRatio,
                Color fillColor,
                Color backgroundColor) {
                bindBase(title, value, string.Empty, fillRatio, fillColor, backgroundColor);
                configureFillArea(
                    new Vector2(0.16f, 0.18f),
                    new Vector2(0.58f, 0.82f),
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f),
                    true);
                setFillRatio(fillRatio);

                titleText.fontSize = 15f;
                valueText.fontSize = 16f;
                detailText.gameObject.SetActive(false);
                extraText.gameObject.SetActive(false);
                titleText.alignment = TextAlignmentOptions.Left;
                valueText.alignment = TextAlignmentOptions.Center;
                titleText.color = DefenseLayerPalette.MainText;
                valueText.color = DefenseLayerPalette.MainText;

                setRect(
                    titleRectTransform,
                    Vector2.zero,
                    new Vector2(0.15f, 1f),
                    new Vector2(12f, 0f),
                    new Vector2(-4f, 0f));
                setRect(
                    valueRectTransform,
                    new Vector2(0.16f, 0.18f),
                    new Vector2(0.58f, 0.82f),
                    Vector2.zero,
                    Vector2.zero);
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
                outline.effectColor = DefenseLayerPalette.Border;
                outline.effectDistance = new Vector2(2f, -2f);

                fillFrameImage = createFillFrameImage(transform);
                fillImage = createFillImage(transform);
                titleText = createText("Title", transform, 12f, FontStyles.Bold, TextAlignmentOptions.Left);
                valueText = createText("Value", transform, 13f, FontStyles.Bold, TextAlignmentOptions.Center);
                detailText = createText("Detail", transform, 11f, FontStyles.Bold, TextAlignmentOptions.Right);
                extraText = createText("Extra", transform, 11f, FontStyles.Bold, TextAlignmentOptions.Right);

                fillFrameRectTransform = (RectTransform)fillFrameImage.transform;
                fillRectTransform = (RectTransform)fillImage.transform;
                titleRectTransform = (RectTransform)titleText.transform;
                valueRectTransform = (RectTransform)valueText.transform;
                detailRectTransform = (RectTransform)detailText.transform;
                extraRectTransform = (RectTransform)extraText.transform;

                setRect(
                    titleRectTransform,
                    new Vector2(0f, 0f),
                    new Vector2(0.34f, 1f),
                    new Vector2(8f, 0f),
                    new Vector2(-4f, 0f));
                setRect(
                    valueRectTransform,
                    new Vector2(0.34f, 0f),
                    new Vector2(0.74f, 1f),
                    new Vector2(2f, 0f),
                    new Vector2(-2f, 0f));
                setRect(
                    detailRectTransform,
                    new Vector2(0.74f, 0f),
                    Vector2.one,
                    new Vector2(4f, 0f),
                    new Vector2(-8f, 0f));
                setRect(
                    extraRectTransform,
                    new Vector2(0.74f, 0f),
                    Vector2.one,
                    new Vector2(4f, 0f),
                    new Vector2(-8f, 0f));
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

            private static Image createFillFrameImage(Transform parent) {
                GameObject fillFrame =
                    new GameObject("FillFrame", typeof(RectTransform), typeof(Image), typeof(Outline));
                fillFrame.transform.SetParent(parent, false);

                Image image = fillFrame.GetComponent<Image>();
                image.color = DefenseLayerPalette.MainBackground;
                image.raycastTarget = false;

                Outline outline = fillFrame.GetComponent<Outline>();
                outline.effectColor = DefenseLayerPalette.OldGold;
                outline.effectDistance = new Vector2(1f, -1f);

                fillFrame.SetActive(false);
                return image;
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
                image.type = Image.Type.Simple;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.raycastTarget = false;
                return image;
            }

            private void bindBase(
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
                setFillRatio(fillRatio);
                gameObject.SetActive(true);
            }

            private void configureFillArea(
                Vector2 anchorMin,
                Vector2 anchorMax,
                Vector2 offsetMin,
                Vector2 offsetMax,
                bool showFrame) {
                fillAreaAnchorMin = anchorMin;
                fillAreaAnchorMax = anchorMax;
                fillAreaOffsetMin = offsetMin;
                fillAreaOffsetMax = offsetMax;

                setRect(fillFrameRectTransform, anchorMin, anchorMax, offsetMin, offsetMax);
                fillFrameImage.gameObject.SetActive(showFrame);
            }

            private void setFillRatio(float fillRatio) {
                float clampedFillRatio = Mathf.Clamp01(fillRatio);
                fillImage.fillAmount = clampedFillRatio;
                fillImage.gameObject.SetActive(clampedFillRatio > 0.001f);

                if (fillRectTransform == null) {
                    fillRectTransform = (RectTransform)fillImage.transform;
                }

                fillRectTransform.anchorMin = fillAreaAnchorMin;
                fillRectTransform.anchorMax = new Vector2(
                    Mathf.Lerp(fillAreaAnchorMin.x, fillAreaAnchorMax.x, clampedFillRatio),
                    fillAreaAnchorMax.y);
                fillRectTransform.offsetMin = fillAreaOffsetMin;
                fillRectTransform.offsetMax = clampedFillRatio > 0.001f
                    ? fillAreaOffsetMax
                    : new Vector2(fillAreaOffsetMin.x, fillAreaOffsetMax.y);
            }

            private static void setRect(
                RectTransform rect,
                Vector2 anchorMin,
                Vector2 anchorMax,
                Vector2 offsetMin,
                Vector2 offsetMax) {
                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
                rect.offsetMin = offsetMin;
                rect.offsetMax = offsetMax;
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
                text.color = DefenseLayerPalette.MainText;
                text.raycastTarget = false;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                return text;
            }
        }

        private sealed class DamagePacketPathView : MonoBehaviour {
            private const int StageBandCount = 4;
            private const int MaxVisiblePackets = VisibleDamagePacketCapacity;
            private const float PacketWidth = DamagePacketValueWidth;
            private const float PacketHeight = 20f;
            private const float IncomingStageBandHeight = 28f;

            private RectTransform rectTransform;
            private Image backgroundImage;
            private TextMeshProUGUI overflowText;
            private readonly DamagePacketValueView[] packetViews = new DamagePacketValueView[MaxVisiblePackets];
            private readonly RectTransform[] stageBandRects = new RectTransform[StageBandCount];
            private readonly List<long> hiddenPacketIds = new();
            private long nextDisplaySequence;

            internal static DamagePacketPathView create(string name, Transform parent) {
                GameObject root = new GameObject(
                    name,
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(Outline),
                    typeof(DamagePacketPathView));
                root.transform.SetParent(parent, false);

                DamagePacketPathView view = root.GetComponent<DamagePacketPathView>();
                view.initialize();
                return view;
            }

            internal void setSize(float width, float height) {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                configureStageBands();
                configureOverflow();
            }

            internal void setAnchoredPosition(Vector2 anchoredPosition) {
                rectTransform.anchoredPosition = anchoredPosition;
            }

            internal void showPanel() {
                gameObject.SetActive(true);
                refreshVisibility();
            }

            internal void showLayer(long packetId, int layerIndex, long damageValue, bool completesPacket) {
                int clampedLayerIndex = Mathf.Clamp(layerIndex, 0, 3);
                gameObject.SetActive(true);

                DamagePacketValueView packetView = claimPacketView(packetId, clampedLayerIndex);
                bool startsNewPacket = !packetView.represents(packetId);
                Vector2 startPosition = startsNewPacket
                    ? getLayerPosition(Math.Max(0, clampedLayerIndex - 1), packetView.getLaneIndex())
                    : packetView.getAnchoredPosition();
                Vector2 targetPosition = getLayerPosition(clampedLayerIndex, packetView.getLaneIndex());
                bool holdForResolvedLayer = completesPacket && clampedLayerIndex == 0;

                packetView.show(
                    packetId,
                    GuardPowerLabelFormatter.format(damageValue),
                    getLayerColor(clampedLayerIndex),
                    startPosition,
                    targetPosition,
                    completesPacket && !holdForResolvedLayer,
                    holdForResolvedLayer,
                    ++nextDisplaySequence,
                    refreshVisibility);
                refreshVisibility();
            }

            private void initialize() {
                rectTransform = (RectTransform)transform;
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);

                backgroundImage = GetComponent<Image>();
                backgroundImage.color = DefenseLayerPalette.Panel;
                backgroundImage.raycastTarget = false;

                Outline outline = GetComponent<Outline>();
                outline.effectColor = DefenseLayerPalette.Border;
                outline.effectDistance = new Vector2(2f, -2f);

                for (int i = 0; i < stageBandRects.Length; i++) {
                    stageBandRects[i] = createStageBand("StageBand" + i, getStageBandColor(i));
                }

                for (int i = 0; i < packetViews.Length; i++) {
                    packetViews[i] = DamagePacketValueView.create("PacketValue" + i, transform, i);
                }

                overflowText = createText(
                    "Overflow",
                    transform,
                    13f,
                    FontStyles.Bold,
                    TextAlignmentOptions.Center);
                overflowText.text = MaxVisiblePackets + "+";
                overflowText.color = DefenseLayerPalette.OldGold;
                overflowText.gameObject.SetActive(false);

                gameObject.SetActive(false);
            }

            private RectTransform createStageBand(string name, Color color) {
                GameObject bandObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Outline));
                bandObject.transform.SetParent(transform, false);

                Image image = bandObject.GetComponent<Image>();
                image.color = color;
                image.raycastTarget = false;

                Outline outline = bandObject.GetComponent<Outline>();
                outline.effectColor = withAlpha(DefenseLayerPalette.Border, 0.32f);
                outline.effectDistance = new Vector2(1f, -1f);

                return (RectTransform)bandObject.transform;
            }

            private void configureStageBands() {
                setStageBand(0, 0f, IncomingStageBandHeight);
                setStageBand(1, -IncomingStageBandHeight, StabilityLayerHeight - IncomingStageBandHeight);
                setStageBand(
                    2,
                    -(StabilityLayerHeight + LayerSpacing),
                    PreparedGuardOverlayView.ReservedHeight);
                setStageBand(
                    3,
                    -(StabilityLayerHeight + LayerSpacing + PreparedGuardOverlayView.ReservedHeight + LayerSpacing),
                    HpLayerHeight);
            }

            private void setStageBand(int index, float topY, float height) {
                RectTransform bandRect = stageBandRects[index];
                bandRect.anchorMin = new Vector2(0f, 1f);
                bandRect.anchorMax = new Vector2(0f, 1f);
                bandRect.pivot = new Vector2(0f, 1f);
                bandRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width);
                bandRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                bandRect.anchoredPosition = new Vector2(0f, topY);
                bandRect.SetAsFirstSibling();
            }

            private void configureOverflow() {
                RectTransform overflowRectTransform = (RectTransform)overflowText.transform;
                overflowRectTransform.anchorMin = new Vector2(0f, 1f);
                overflowRectTransform.anchorMax = new Vector2(0f, 1f);
                overflowRectTransform.pivot = new Vector2(0.5f, 0.5f);
                overflowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 38f);
                overflowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 22f);
                overflowRectTransform.anchoredPosition = new Vector2(
                    rectTransform.rect.width * 0.5f,
                    -rectTransform.rect.height + 13f);
            }

            private DamagePacketValueView claimPacketView(long packetId, int layerIndex) {
                removeHiddenPacketId(packetId);

                for (int i = 0; i < packetViews.Length; i++) {
                    if (packetViews[i].represents(packetId)) {
                        return packetViews[i];
                    }
                }

                if (layerIndex > 0 && tryGetOldestAwaitingResolvedLayer(out DamagePacketValueView awaitingView)) {
                    return awaitingView;
                }

                for (int i = 0; i < packetViews.Length; i++) {
                    if (!packetViews[i].isActive()) {
                        return packetViews[i];
                    }
                }

                DamagePacketValueView reusableView = getOldestActivePacketView();
                addHiddenPacketId(reusableView.getPacketId());
                reusableView.forceHide();
                return reusableView;
            }

            private void refreshVisibility() {
                bool hasAnyActivePacket = false;
                for (int i = 0; i < packetViews.Length; i++) {
                    if (!packetViews[i].isActive()) {
                        continue;
                    }

                    hasAnyActivePacket = true;
                    break;
                }

                if (!hasAnyActivePacket) {
                    hiddenPacketIds.Clear();
                }

                overflowText.gameObject.SetActive(hasAnyActivePacket && hiddenPacketIds.Count > 0);
                overflowText.transform.SetAsLastSibling();
                gameObject.SetActive(true);
            }

            private bool tryGetOldestAwaitingResolvedLayer(out DamagePacketValueView awaitingView) {
                awaitingView = null;
                for (int i = 0; i < packetViews.Length; i++) {
                    if (!packetViews[i].isAwaitingResolvedLayer()) {
                        continue;
                    }

                    if (awaitingView == null
                        || packetViews[i].getDisplaySequence() < awaitingView.getDisplaySequence()) {
                        awaitingView = packetViews[i];
                    }
                }

                return awaitingView != null;
            }

            private DamagePacketValueView getOldestActivePacketView() {
                DamagePacketValueView oldest = packetViews[0];
                for (int i = 1; i < packetViews.Length; i++) {
                    if (packetViews[i].getDisplaySequence() < oldest.getDisplaySequence()) {
                        oldest = packetViews[i];
                    }
                }

                return oldest;
            }

            private void addHiddenPacketId(long packetId) {
                if (packetId <= 0 || hiddenPacketIds.Contains(packetId)) {
                    return;
                }

                hiddenPacketIds.Add(packetId);
            }

            private void removeHiddenPacketId(long packetId) {
                for (int i = hiddenPacketIds.Count - 1; i >= 0; i--) {
                    if (hiddenPacketIds[i] == packetId) {
                        hiddenPacketIds.RemoveAt(i);
                    }
                }
            }

            private Vector2 getLayerPosition(int layerIndex, int laneIndex) {
                float x = getLaneX(laneIndex);
                switch (layerIndex) {
                    case 0:
                        return new Vector2(x, -18f);
                    case 1:
                        return new Vector2(x, -(StabilityLayerHeight + LayerSpacing * 0.5f));
                    case 2:
                        return new Vector2(
                            x,
                            -(StabilityLayerHeight + LayerSpacing + PreparedGuardOverlayView.ReservedHeight * 0.5f));
                    case 3:
                        return new Vector2(
                            x,
                            -(StabilityLayerHeight
                              + LayerSpacing
                              + PreparedGuardOverlayView.ReservedHeight
                              + LayerSpacing
                              + HpLayerHeight * 0.5f));
                    default:
                        return new Vector2(x, 16f);
                }
            }

            private float getLaneX(int laneIndex) {
                float width = rectTransform.rect.width;
                float groupWidth = MaxVisiblePackets * PacketWidth
                                   + (MaxVisiblePackets - 1) * DamagePacketValueGap;
                float startX = Mathf.Max(
                    PacketLaneHorizontalPadding,
                    (width - groupWidth) * 0.5f);
                return startX + PacketWidth * 0.5f + laneIndex * (PacketWidth + DamagePacketValueGap);
            }

            private static Color getLayerColor(int layerIndex) {
                switch (layerIndex) {
                    case 1:
                        return new Color(0.36f, 0.86f, 1f, 1f);
                    case 2:
                        return new Color(0.38f, 0.95f, 0.82f, 1f);
                    case 3:
                        return new Color(1f, 0.36f, 0.32f, 1f);
                    default:
                        return new Color(1f, 0.82f, 0.24f, 1f);
                }
            }

            private static Color getStageBandColor(int stageIndex) {
                switch (stageIndex) {
                    case 0:
                        return withAlpha(DefenseLayerPalette.DefenseBlue, 0.12f);
                    case 1:
                        return withAlpha(DefenseLayerPalette.HealingGreen, 0.09f);
                    case 2:
                        return withAlpha(DefenseLayerPalette.PowerRed, 0.1f);
                    default:
                        return withAlpha(DefenseLayerPalette.PanelRaised, 0.1f);
                }
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

            private sealed class DamagePacketValueView {
                private readonly int laneIndex;
                private GameObject gameObject;
                private RectTransform rectTransform;
                private CanvasGroup canvasGroup;
                private TextMeshProUGUI valueText;
                private Sequence sequence;
                private long activePacketId;
                private long displaySequence;
                private bool active;
                private bool awaitingResolvedLayer;

                private DamagePacketValueView(int laneIndex) {
                    this.laneIndex = laneIndex;
                }

                internal static DamagePacketValueView create(string name, Transform parent, int laneIndex) {
                    GameObject root = new GameObject(
                        name,
                        typeof(RectTransform),
                        typeof(Image),
                        typeof(Outline),
                        typeof(CanvasGroup));
                    root.transform.SetParent(parent, false);

                    DamagePacketValueView view = new DamagePacketValueView(laneIndex);
                    view.initialize(root);
                    return view;
                }

                internal int getLaneIndex() {
                    return laneIndex;
                }

                internal bool isActive() {
                    return active;
                }

                internal bool represents(long packetId) {
                    return active && activePacketId == packetId;
                }

                internal bool isAwaitingResolvedLayer() {
                    return active && awaitingResolvedLayer;
                }

                internal long getPacketId() {
                    return activePacketId;
                }

                internal long getDisplaySequence() {
                    return displaySequence;
                }

                internal Vector2 getAnchoredPosition() {
                    return rectTransform.anchoredPosition;
                }

                internal void show(
                    long packetId,
                    string value,
                    Color color,
                    Vector2 startPosition,
                    Vector2 targetPosition,
                    bool fadesOnCompletion,
                    bool awaitingResolvedLayer,
                    long newDisplaySequence,
                    Action onVisibilityChanged) {
                    if (sequence != null && sequence.IsActive()) {
                        sequence.Kill();
                    }

                    activePacketId = packetId;
                    displaySequence = newDisplaySequence;
                    active = true;
                    this.awaitingResolvedLayer = awaitingResolvedLayer;
                    valueText.text = value;
                    valueText.color = color;
                    canvasGroup.alpha = 1f;
                    rectTransform.anchoredPosition = startPosition;
                    rectTransform.localScale = Vector3.one;
                    gameObject.SetActive(true);
                    rectTransform.SetAsLastSibling();

                    sequence = DOTween.Sequence();
                    sequence.Append(rectTransform.DOAnchorPos(targetPosition, 0.46f).SetEase(Ease.InOutSine));
                    sequence.Join(rectTransform.DOPunchScale(new Vector3(0.08f, 0.08f, 0f), 0.22f, 4, 0.45f));

                    if (fadesOnCompletion) {
                        sequence.AppendInterval(0.25f);
                        sequence.Append(canvasGroup.DOFade(0f, 0.2f));
                        sequence.AppendCallback(() => {
                            active = false;
                            this.awaitingResolvedLayer = false;
                            activePacketId = 0;
                            displaySequence = 0;
                            gameObject.SetActive(false);
                            onVisibilityChanged?.Invoke();
                        });
                    }
                }

                internal void forceHide() {
                    if (sequence != null && sequence.IsActive()) {
                        sequence.Kill();
                    }

                    active = false;
                    awaitingResolvedLayer = false;
                    activePacketId = 0;
                    displaySequence = 0;
                    canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                }

                private void initialize(GameObject root) {
                    gameObject = root;
                    rectTransform = (RectTransform)root.transform;
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PacketWidth);
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PacketHeight);

                    Image backgroundImage = root.GetComponent<Image>();
                    backgroundImage.color = new Color(0.06f, 0.07f, 0.09f, 0.9f);
                    backgroundImage.raycastTarget = false;

                    Outline outline = root.GetComponent<Outline>();
                    outline.effectColor = DefenseLayerPalette.OldGold;
                    outline.effectDistance = new Vector2(1f, -1f);

                    canvasGroup = root.GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0f;

                    valueText = createText(
                        "Value",
                        root.transform,
                        14f,
                        FontStyles.Bold,
                        TextAlignmentOptions.Center);
                    RectTransform valueRect = (RectTransform)valueText.transform;
                    valueRect.anchorMin = Vector2.zero;
                    valueRect.anchorMax = Vector2.one;
                    valueRect.offsetMin = new Vector2(2f, 0f);
                    valueRect.offsetMax = new Vector2(-2f, 0f);

                    root.SetActive(false);
                }
            }
        }
    }
}