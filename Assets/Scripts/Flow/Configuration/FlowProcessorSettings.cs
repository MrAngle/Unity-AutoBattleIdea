using System;

namespace MageFactory.Flow.Configuration {
    public sealed class FlowProcessorSettings {
        private readonly int maxStepsPerSlice;
        private readonly FlowCastTimeMode castTimeMode;

        public FlowProcessorSettings(int maxStepsPerSlice, FlowCastTimeMode castTimeMode) {
            if (maxStepsPerSlice < 1) {
                throw new ArgumentOutOfRangeException(
                    nameof(maxStepsPerSlice),
                    "Flow processor must be allowed to process at least one step per tick.");
            }

            if (!Enum.IsDefined(typeof(FlowCastTimeMode), castTimeMode)) {
                throw new ArgumentOutOfRangeException(
                    nameof(castTimeMode),
                    castTimeMode,
                    "Flow cast time mode must be defined.");
            }

            this.maxStepsPerSlice = maxStepsPerSlice;
            this.castTimeMode = castTimeMode;
        }

        public int getMaxStepsPerSlice() {
            return maxStepsPerSlice;
        }

        public FlowCastTimeMode getCastTimeMode() {
            return castTimeMode;
        }
    }
}