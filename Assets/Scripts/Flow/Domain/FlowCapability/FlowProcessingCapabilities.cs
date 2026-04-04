using MageFactory.Flow.Domain.Service;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingCapabilities {
        private readonly FlowProcessingCommandBus flowProcessingCommandBus;
        private readonly FlowProcessingQueries flowProcessingQueries;

        public FlowProcessingCapabilities(FlowContext flowContext, ActionContextFactory actionContextFactory) {
            flowProcessingCommandBus = new FlowProcessingCommandBus(flowContext);
            flowProcessingQueries = new FlowProcessingQueries(flowContext, actionContextFactory);
        }

        public FlowProcessingCommandBus command() {
            return flowProcessingCommandBus;
        }

        public FlowProcessingQueries query() {
            return flowProcessingQueries;
        }
    }
}