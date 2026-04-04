using MageFactory.Flow.Domain.Service;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowCapabilities {
        private readonly FlowCommandBus flowCommandBus;
        private readonly FlowQueries flowQueries;

        public FlowCapabilities(FlowContext flowContext, ActionContextFactory actionContextFactory) {
            flowCommandBus = new FlowCommandBus(flowContext);
            flowQueries = new FlowQueries(flowContext, actionContextFactory);
        }

        public FlowCommandBus command() {
            return flowCommandBus;
        }

        public FlowQueries query() {
            return flowQueries;
        }
    }
}