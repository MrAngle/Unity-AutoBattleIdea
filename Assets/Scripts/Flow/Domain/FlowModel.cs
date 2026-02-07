using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    internal class FlowModel {
        private readonly FlowSeed flowSeed;
        private readonly FlowPayload flowPayload;
        private readonly FlowContext flowContext;

        internal FlowModel(FlowSeed flowSeed, FlowContext flowContext) {
            this.flowSeed = flowSeed;
            this.flowPayload = new FlowPayload(flowSeed.power()); // for now
            this.flowContext = flowContext;
        }

        internal FlowPayload getFlowPayload() {
            return flowPayload;
        }

        internal FlowContext getFlowContext() {
            return flowContext;
        }

        internal void addPower(PowerAmount damageAmount) {
            flowPayload.add(damageAmount);
        }
    }
}