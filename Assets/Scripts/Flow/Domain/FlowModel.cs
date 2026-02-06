using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    public class FlowModel {
        public FlowModel(FlowSeed flowSeed, FlowContext flowContext) {
            FlowSeed = flowSeed;
            FlowPayload = new FlowPayload(flowSeed.Power());
            FlowContext = flowContext;
        }

        public FlowSeed FlowSeed { get; }
        public FlowPayload FlowPayload { get; }
        public FlowContext FlowContext { get; }

        public void AddPower(PowerAmount damageAmount) {
            FlowPayload.Add(damageAmount);
        }
    }
}