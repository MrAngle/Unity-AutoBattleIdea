using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.Service;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingCapabilities {
        private readonly FlowProcessingCommandBus flowProcessingCommandBus;
        private readonly FlowProcessingQueries flowProcessingQueries;

        public FlowProcessingCapabilities(FlowContext flowContext, ActionContextFactory actionContextFactory,
                                          IFlowCapabilities flowCapabilities) {
            flowProcessingCommandBus = new FlowProcessingCommandBus(flowContext);
            flowProcessingQueries = new FlowProcessingQueries(flowContext, actionContextFactory, flowCapabilities);
        }

        public FlowProcessingCommandBus command() {
            return flowProcessingCommandBus;
        }

        public FlowProcessingQueries query() {
            return flowProcessingQueries;
        }
    }
}