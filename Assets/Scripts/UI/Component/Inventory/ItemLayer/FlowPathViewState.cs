using System;
using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public readonly struct FlowPathViewState {
        private readonly Id<ActiveFlowId> flowId;
        private readonly int flowVisualIndex;
        private readonly IReadOnlyList<ItemFlowProcessingSlot> processingPath;
        private readonly float currentProgressRatio;

        public FlowPathViewState(
            Id<ActiveFlowId> flowId,
            int flowVisualIndex,
            IReadOnlyList<ItemFlowProcessingSlot> processingPath,
            float currentProgressRatio) {
            if (flowVisualIndex < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(flowVisualIndex),
                    flowVisualIndex,
                    "Flow visual index cannot be negative.");
            }

            if (float.IsNaN(currentProgressRatio)) {
                throw new ArgumentOutOfRangeException(
                    nameof(currentProgressRatio),
                    currentProgressRatio,
                    "Flow progress ratio cannot be NaN.");
            }

            this.flowId = NullGuard.ValidIdOrThrow(flowId);
            this.flowVisualIndex = flowVisualIndex;
            this.processingPath = NullGuard.NotNullOrThrow(processingPath);
            this.currentProgressRatio = currentProgressRatio;
        }

        public Id<ActiveFlowId> getFlowId() {
            return flowId;
        }

        public int getFlowVisualIndex() {
            return flowVisualIndex;
        }

        public IReadOnlyList<ItemFlowProcessingSlot> getProcessingPath() {
            return processingPath;
        }

        public float getCurrentProgressRatio() {
            return currentProgressRatio;
        }
    }
}