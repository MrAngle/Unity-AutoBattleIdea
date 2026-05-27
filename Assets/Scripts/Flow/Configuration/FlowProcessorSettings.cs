namespace MageFactory.Flow.Configuration {
    public sealed class FlowProcessorSettings {
        public int maxStepsPerSlice { get; }

        public FlowProcessorSettings(int maxStepsPerSlice = 64) {
            // verify
            this.maxStepsPerSlice = maxStepsPerSlice < 1 ? 1 : maxStepsPerSlice;
        }
    }
}