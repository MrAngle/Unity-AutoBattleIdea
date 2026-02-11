using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    internal class FlowModel {
        private readonly FlowPayload flowPayload;
        private readonly FlowContext flowContext;

        internal FlowModel(FlowContext flowContext) {
            this.flowPayload = new FlowPayload();
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