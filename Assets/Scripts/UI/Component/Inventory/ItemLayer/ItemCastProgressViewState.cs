using System;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public readonly struct ItemCastProgressViewState {
        private readonly int localRow;
        private readonly float progressRatio;

        public ItemCastProgressViewState(
            int localRow,
            float progressRatio) {
            if (float.IsNaN(progressRatio)) {
                throw new ArgumentOutOfRangeException(
                    nameof(progressRatio),
                    progressRatio,
                    "Cast progress ratio cannot be NaN.");
            }

            this.localRow = localRow;
            this.progressRatio = progressRatio;
        }

        public int getLocalRow() {
            return localRow;
        }

        public float getProgressRatio() {
            return progressRatio;
        }
    }
}