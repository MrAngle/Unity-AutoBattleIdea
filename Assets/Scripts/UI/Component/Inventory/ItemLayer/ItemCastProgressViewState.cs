using System;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public readonly struct ItemCastProgressViewState {
        private readonly Id<ActiveFlowId> flowId;
        private readonly int localRow;
        private readonly float progressRatio;
        private readonly int flowVisualIndex;

        public ItemCastProgressViewState(
            Id<ActiveFlowId> flowId,
            int localRow,
            float progressRatio,
            int flowVisualIndex) {
            if (float.IsNaN(progressRatio)) {
                throw new ArgumentOutOfRangeException(
                    nameof(progressRatio),
                    progressRatio,
                    "Cast progress ratio cannot be NaN.");
            }

            if (flowVisualIndex < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(flowVisualIndex),
                    flowVisualIndex,
                    "Flow visual index cannot be negative.");
            }

            this.flowId = NullGuard.ValidIdOrThrow(flowId);
            this.localRow = localRow;
            this.progressRatio = progressRatio;
            this.flowVisualIndex = flowVisualIndex;
        }

        public Id<ActiveFlowId> getFlowId() {
            return flowId;
        }

        public int getLocalRow() {
            return localRow;
        }

        public float getProgressRatio() {
            return progressRatio;
        }

        public int getFlowVisualIndex() {
            return flowVisualIndex;
        }
    }
}