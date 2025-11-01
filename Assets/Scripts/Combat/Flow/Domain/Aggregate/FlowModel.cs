namespace Combat.Flow.Domain.Aggregate {
    public class FlowModel
    {
        public FlowSeed FlowSeed { get; }
        public FlowPayload FlowPayload { get; }
        public FlowContext FlowContext { get; }

        public FlowModel(FlowSeed flowSeed, FlowContext flowContext)
        {
            FlowSeed = flowSeed;
            FlowPayload = new FlowPayload(flowSeed.Power());
            FlowContext = flowContext;
        }
    }
}