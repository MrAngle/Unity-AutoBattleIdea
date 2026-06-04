using UnityEngine;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal static class FlowVisualPalette {
        private static readonly Color[] Colors = {
            new(0.20f, 0.84f, 1.00f, 0.88f),
            new(1.00f, 0.56f, 0.24f, 0.88f),
            new(0.48f, 1.00f, 0.48f, 0.88f),
            new(1.00f, 0.36f, 0.66f, 0.88f),
            new(0.72f, 0.62f, 1.00f, 0.88f),
            new(1.00f, 0.90f, 0.30f, 0.88f),
            new(0.36f, 0.98f, 0.78f, 0.88f),
            new(0.95f, 0.42f, 1.00f, 0.88f)
        };

        internal static Color getColor(int visualIndex) {
            if (visualIndex < 0) {
                visualIndex = 0;
            }

            return Colors[visualIndex % Colors.Length];
        }

        internal static Color withAlpha(Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}