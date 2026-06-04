using UnityEngine;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal enum InventoryItemVisualState {
        Normal,
        Focused,
        Related,
        Dimmed,
        StaleRelated
    }

    internal readonly struct InventoryItemVisualStyle {
        private readonly Color fillColor;
        private readonly bool outlineEnabled;
        private readonly Color outlineColor;
        private readonly Vector2 outlineDistance;

        private InventoryItemVisualStyle(
            Color fillColor,
            bool outlineEnabled,
            Color outlineColor,
            Vector2 outlineDistance) {
            this.fillColor = fillColor;
            this.outlineEnabled = outlineEnabled;
            this.outlineColor = outlineColor;
            this.outlineDistance = outlineDistance;
        }

        internal Color getFillColor() {
            return fillColor;
        }

        internal bool isOutlineEnabled() {
            return outlineEnabled;
        }

        internal Color getOutlineColor() {
            return outlineColor;
        }

        internal Vector2 getOutlineDistance() {
            return outlineDistance;
        }

        internal static InventoryItemVisualStyle from(Color baseColor, InventoryItemVisualState state) {
            switch (state) {
                case InventoryItemVisualState.Focused:
                    return new InventoryItemVisualStyle(
                        withAlpha(brighten(baseColor, 1.18f), 1f),
                        true,
                        new Color(0.92f, 1f, 1f, 0.95f),
                        new Vector2(3f, -3f));
                case InventoryItemVisualState.Related:
                    return new InventoryItemVisualStyle(
                        withAlpha(brighten(baseColor, 1.05f), 0.92f),
                        true,
                        new Color(0.64f, 0.86f, 1f, 0.58f),
                        new Vector2(1.5f, -1.5f));
                case InventoryItemVisualState.Dimmed:
                    return new InventoryItemVisualStyle(
                        withAlpha(desaturate(baseColor, 0.42f), 0.28f),
                        true,
                        new Color(0.14f, 0.18f, 0.24f, 0.72f),
                        new Vector2(1.5f, -1.5f));
                case InventoryItemVisualState.StaleRelated:
                    return new InventoryItemVisualStyle(
                        withAlpha(desaturate(baseColor, 0.7f), 0.48f),
                        true,
                        new Color(0.35f, 0.48f, 0.58f, 0.52f),
                        new Vector2(1.5f, -1.5f));
                default:
                    return new InventoryItemVisualStyle(
                        baseColor,
                        true,
                        new Color(0.18f, 0.24f, 0.32f, 0.72f),
                        new Vector2(1.5f, -1.5f));
            }
        }

        private static Color withAlpha(Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }

        private static Color brighten(Color color, float multiplier) {
            return new Color(
                Mathf.Clamp01(color.r * multiplier),
                Mathf.Clamp01(color.g * multiplier),
                Mathf.Clamp01(color.b * multiplier),
                color.a);
        }

        private static Color desaturate(Color color, float saturation) {
            float gray = color.grayscale;
            return new Color(
                Mathf.Lerp(gray, color.r, saturation),
                Mathf.Lerp(gray, color.g, saturation),
                Mathf.Lerp(gray, color.b, saturation),
                color.a);
        }
    }
}