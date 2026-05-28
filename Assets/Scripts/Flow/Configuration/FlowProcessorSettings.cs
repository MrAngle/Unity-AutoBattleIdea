using System;

namespace MageFactory.Flow.Configuration {
    public sealed class FlowProcessorSettings {
        private readonly int maxStepsPerSlice;

        public FlowProcessorSettings(int maxStepsPerSlice) {
            if (maxStepsPerSlice < 1) {
                throw new ArgumentOutOfRangeException(
                    nameof(maxStepsPerSlice),
                    "Flow processor must be allowed to process at least one step per tick.");
            }

            this.maxStepsPerSlice = maxStepsPerSlice;
        }

        public int getMaxStepsPerSlice() {
            return maxStepsPerSlice;
        }
    }
}