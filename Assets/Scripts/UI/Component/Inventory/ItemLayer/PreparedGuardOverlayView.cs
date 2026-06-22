using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Shared.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("MageFactory.Tests")]

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal sealed class PreparedGuardOverlayView : MonoBehaviour {
        private const int MaxRows = 2;
        private const float IconWidth = 50f;
        private const float IconHeight = 62f;
        private const float IconSpacing = 6f;
        private const float RootPadding = 4f;

        private readonly List<PreparedGuardIconView> iconViews = new();
        private PreparedGuardTooltipView tooltipView;
        private RectTransform rectTransform;

        internal static PreparedGuardOverlayView create(Transform parent) {
            NullGuard.NotNullOrThrow(parent);

            GameObject root = new GameObject(
                "PreparedGuardOverlay",
                typeof(RectTransform),
                typeof(PreparedGuardOverlayView));
            root.transform.SetParent(parent, false);

            PreparedGuardOverlayView view = root.GetComponent<PreparedGuardOverlayView>();
            view.initialize();
            return view;
        }

        internal void printGuards(
            IReadOnlyList<PreparedGuardState> guardStates,
            ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            IReadOnlyList<PreparedGuardState> validGuardStates = NullGuard.NotNullOrThrow(guardStates);

            if (validGuardStates.Count == 0) {
                hideAll();
                return;
            }

            gameObject.SetActive(true);
            configureRoot(inventoryGridInfo);

            int columns = calculateColumns(inventoryGridInfo);
            int visibleSlots = Math.Min(validGuardStates.Count, columns * MaxRows);
            bool hasOverflow = validGuardStates.Count > visibleSlots;
            int visibleGuardCount = hasOverflow
                ? Math.Max(0, visibleSlots - 1)
                : visibleSlots;

            ensureIconCount(visibleSlots);

            int iconIndex = 0;
            for (; iconIndex < visibleGuardCount; iconIndex++) {
                PreparedGuardIconView iconView = iconViews[iconIndex];
                positionIcon(iconView, iconIndex, columns);
                iconView.bind(validGuardStates[iconIndex]);
            }

            if (hasOverflow && visibleSlots > 0) {
                PreparedGuardIconView iconView = iconViews[iconIndex];
                positionIcon(iconView, iconIndex, columns);
                iconView.bindOverflow(validGuardStates.Count - visibleGuardCount);
                iconIndex++;
            }

            for (; iconIndex < iconViews.Count; iconIndex++) {
                iconViews[iconIndex].hide();
            }
        }

        internal void hideAll() {
            if (tooltipView != null) {
                tooltipView.hide();
            }

            for (int i = 0; i < iconViews.Count; i++) {
                iconViews[i].hide();
            }

            gameObject.SetActive(false);
        }

        internal bool tryGetGuardCenterInParent(Id<GuardId> guardId, out Vector2 center) {
            for (int i = 0; i < iconViews.Count; i++) {
                PreparedGuardIconView iconView = iconViews[i];
                if (!iconView.tryGetCenterFor(guardId, out Vector2 iconCenter)) {
                    continue;
                }

                center = rectTransform.anchoredPosition + iconCenter;
                return true;
            }

            center = getOverlayCenterInParent();
            return false;
        }

        internal Vector2 getOverlayCenterInParent() {
            return rectTransform.anchoredPosition + new Vector2(
                rectTransform.rect.width * 0.5f,
                -rectTransform.rect.height * 0.5f);
        }

        internal void showGuardAbsorbedVisual(Id<GuardId> guardId, long blockedDamage) {
            if (!tryGetIconFor(guardId, out PreparedGuardIconView iconView)) {
                return;
            }

            iconView.playAbsorbVisual(blockedDamage);
        }

        internal void showGuardReplacedVisual(Id<GuardId> guardId, long replacedGuardPower) {
            if (!tryGetIconFor(guardId, out PreparedGuardIconView iconView)) {
                return;
            }

            iconView.playReplacementVisual(replacedGuardPower);
        }

        private void initialize() {
            rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = Vector2.zero;

            tooltipView = PreparedGuardTooltipView.create(transform);
            hideAll();
        }

        private void configureRoot(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float width = calculateInventoryWidth(inventoryGridInfo);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                MaxRows * IconHeight + (MaxRows - 1) * IconSpacing);
            rectTransform.anchoredPosition = new Vector2(
                RootPadding,
                rectTransform.rect.height + RootPadding);
        }

        private static float calculateInventoryWidth(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            int widthCells = Math.Max(1, inventoryGridInfo.WidthCellsNumber);
            return widthCells * inventoryGridInfo.CellSize.x
                   + Math.Max(0, widthCells - 1) * inventoryGridInfo.Spacing.x;
        }

        private static int calculateColumns(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float inventoryWidth = calculateInventoryWidth(inventoryGridInfo);
            int columns = Mathf.FloorToInt((inventoryWidth + IconSpacing) / (IconWidth + IconSpacing));
            return Math.Max(1, columns);
        }

        private void ensureIconCount(int count) {
            while (iconViews.Count < count) {
                iconViews.Add(PreparedGuardIconView.create(transform, tooltipView));
            }
        }

        private bool tryGetIconFor(Id<GuardId> guardId, out PreparedGuardIconView matchingIconView) {
            for (int i = 0; i < iconViews.Count; i++) {
                PreparedGuardIconView iconView = iconViews[i];
                if (!iconView.represents(guardId)) {
                    continue;
                }

                matchingIconView = iconView;
                return true;
            }

            matchingIconView = null;
            return false;
        }

        private static void positionIcon(PreparedGuardIconView iconView, int iconIndex, int columns) {
            int row = iconIndex / columns;
            int column = iconIndex % columns;
            iconView.setAnchoredPosition(new Vector2(
                column * (IconWidth + IconSpacing),
                -row * (IconHeight + IconSpacing)));
        }

        private sealed class PreparedGuardIconView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
            private PreparedGuardTooltipView tooltipView;
            private RectTransform rectTransform;
            private Image backgroundImage;
            private TextMeshProUGUI glyphText;
            private TextMeshProUGUI powerText;
            private PreparedGuardState guardState;
            private bool hasGuardState;
            private string overflowTooltip;
            private Sequence visualSequence;

            internal static PreparedGuardIconView create(
                Transform parent,
                PreparedGuardTooltipView tooltipView) {
                GameObject root = new GameObject(
                    "PreparedGuardIcon",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(Outline),
                    typeof(PreparedGuardIconView));
                root.transform.SetParent(parent, false);

                PreparedGuardIconView view = root.GetComponent<PreparedGuardIconView>();
                view.initialize(tooltipView);
                return view;
            }

            internal void bind(PreparedGuardState state) {
                guardState = state;
                hasGuardState = true;
                overflowTooltip = null;

                glyphText.text = "\u26E8";
                powerText.text = GuardPowerLabelFormatter.format(state.getGuardPower().getPower());
                backgroundImage.color = new Color(0.07f, 0.2f, 0.22f, 0.86f);
                gameObject.SetActive(true);
            }

            internal void bindOverflow(int hiddenGuardCount) {
                hasGuardState = false;
                overflowTooltip = $"{hiddenGuardCount} more guards are hidden by the inventory-width display limit.";

                glyphText.text = "...";
                powerText.text = "+" + hiddenGuardCount;
                backgroundImage.color = new Color(0.08f, 0.08f, 0.1f, 0.84f);
                gameObject.SetActive(true);
            }

            internal void hide() {
                hasGuardState = false;
                overflowTooltip = null;
                gameObject.SetActive(false);
            }

            internal void setAnchoredPosition(Vector2 anchoredPosition) {
                rectTransform.anchoredPosition = anchoredPosition;
            }

            internal bool tryGetCenterFor(Id<GuardId> guardId, out Vector2 center) {
                if (represents(guardId)) {
                    center = rectTransform.anchoredPosition + new Vector2(
                        rectTransform.rect.width * 0.5f,
                        -rectTransform.rect.height * 0.5f);
                    return true;
                }

                center = default;
                return false;
            }

            internal bool represents(Id<GuardId> guardId) {
                return gameObject.activeSelf
                       && hasGuardState
                       && guardState.getGuardId() == guardId;
            }

            internal void playAbsorbVisual(long blockedDamage) {
                playPunch(new Color(0.36f, 0.95f, 0.82f, 1f));
                PopupManager.Instance?.Show(
                    this,
                    "G -" + GuardPowerLabelFormatter.format(blockedDamage),
                    new Color(0.36f, 0.95f, 0.82f, 1f),
                    moveY: 34f,
                    duration: 0.7f);
            }

            internal void playReplacementVisual(long replacedGuardPower) {
                playPunch(new Color(1f, 0.43f, 0.22f, 1f));
                PopupManager.Instance?.Show(
                    this,
                    "push " + GuardPowerLabelFormatter.format(replacedGuardPower),
                    new Color(1f, 0.43f, 0.22f, 1f),
                    moveY: 34f,
                    duration: 0.75f);
            }

            public void OnPointerEnter(PointerEventData eventData) {
                if (hasGuardState) {
                    tooltipView.show(guardState, rectTransform);
                    return;
                }

                if (!string.IsNullOrEmpty(overflowTooltip)) {
                    tooltipView.showText(overflowTooltip, rectTransform);
                }
            }

            public void OnPointerExit(PointerEventData eventData) {
                tooltipView.hide();
            }

            private void initialize(PreparedGuardTooltipView preparedGuardTooltipView) {
                tooltipView = NullGuard.NotNullOrThrow(preparedGuardTooltipView);
                rectTransform = (RectTransform)transform;
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, IconWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, IconHeight);

                backgroundImage = GetComponent<Image>();
                backgroundImage.raycastTarget = true;

                Outline outline = GetComponent<Outline>();
                outline.effectColor = new Color(0.38f, 0.95f, 0.82f, 0.55f);
                outline.effectDistance = new Vector2(1f, -1f);

                glyphText = createText("Glyph", transform, 22f, FontStyles.Bold);
                RectTransform glyphRect = (RectTransform)glyphText.transform;
                glyphRect.anchorMin = new Vector2(0f, 0.46f);
                glyphRect.anchorMax = Vector2.one;
                glyphRect.offsetMin = new Vector2(0f, -1f);
                glyphRect.offsetMax = Vector2.zero;
                glyphText.text = "\u26E8";

                powerText = createText("Power", transform, 15f, FontStyles.Bold);
                RectTransform powerRect = (RectTransform)powerText.transform;
                powerRect.anchorMin = Vector2.zero;
                powerRect.anchorMax = new Vector2(1f, 0.44f);
                powerRect.offsetMin = new Vector2(2f, 1f);
                powerRect.offsetMax = new Vector2(-2f, -1f);
                powerText.color = new Color(0.84f, 1f, 0.95f, 1f);
            }

            private void playPunch(Color color) {
                if (visualSequence != null && visualSequence.IsActive()) {
                    visualSequence.Kill();
                }

                Color originalColor = backgroundImage.color;
                visualSequence = DOTween.Sequence();
                visualSequence.Append(backgroundImage.DOColor(color, 0.08f));
                visualSequence.Join(rectTransform.DOPunchScale(new Vector3(0.16f, 0.1f, 0f), 0.22f, 6, 0.6f));
                visualSequence.Append(backgroundImage.DOColor(originalColor, 0.18f));
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
        }

        private sealed class PreparedGuardTooltipView : MonoBehaviour {
            private const float TooltipWidth = 270f;
            private const float TooltipHeight = 124f;

            private RectTransform rectTransform;
            private TextMeshProUGUI tooltipText;

            internal static PreparedGuardTooltipView create(Transform parent) {
                GameObject root = new GameObject(
                    "PreparedGuardTooltip",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(CanvasGroup),
                    typeof(PreparedGuardTooltipView));
                root.transform.SetParent(parent, false);

                PreparedGuardTooltipView view = root.GetComponent<PreparedGuardTooltipView>();
                view.initialize();
                return view;
            }

            internal void show(PreparedGuardState guardState, RectTransform targetRectTransform) {
                showText(buildTooltip(guardState), targetRectTransform);
            }

            internal void showText(string text, RectTransform targetRectTransform) {
                tooltipText.text = text;
                rectTransform.anchoredPosition = targetRectTransform.anchoredPosition
                                                 + new Vector2(0f, -TooltipHeight - IconHeight - 8f);
                gameObject.SetActive(true);
            }

            internal void hide() {
                gameObject.SetActive(false);
            }

            private void initialize() {
                rectTransform = (RectTransform)transform;
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TooltipWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TooltipHeight);

                Image background = GetComponent<Image>();
                background.color = new Color(0.03f, 0.05f, 0.07f, 0.94f);
                background.raycastTarget = false;

                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                tooltipText = createTooltipText(transform);
                hide();
            }

            private static TextMeshProUGUI createTooltipText(Transform parent) {
                GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(parent, false);

                RectTransform textRectTransform = (RectTransform)textObject.transform;
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = new Vector2(8f, 8f);
                textRectTransform.offsetMax = new Vector2(-8f, -8f);

                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
                text.alignment = TextAlignmentOptions.TopLeft;
                text.fontSize = 11f;
                text.color = new Color(0.9f, 1f, 0.96f, 1f);
                text.textWrappingMode = TextWrappingModes.Normal;
                text.raycastTarget = false;
                return text;
            }

            private static string buildTooltip(PreparedGuardState guardState) {
                long guardPower = guardState.getGuardPower().getPower();
                long smallDamage = Math.Max(1, guardPower / 2);
                long matchingDamage = Math.Max(1, guardPower);
                long largeDamage = guardPower > long.MaxValue / 2
                    ? long.MaxValue
                    : Math.Max(1, guardPower * 2);

                return "Guard Power: " + guardPower + "\n"
                       + "Against " + smallDamage + " damage: blocks ~"
                       + GuardMitigationCalculator.calculateBlockedDamage(guardPower, smallDamage) + "\n"
                       + "Against " + matchingDamage + " damage: blocks ~"
                       + GuardMitigationCalculator.calculateBlockedDamage(guardPower, matchingDamage) + "\n"
                       + "Against " + largeDamage + " damage: blocks ~"
                       + GuardMitigationCalculator.calculateBlockedDamage(guardPower, largeDamage) + "\n"
                       + "Each hit strains this guard by blocked damage.";
            }
        }
    }
}