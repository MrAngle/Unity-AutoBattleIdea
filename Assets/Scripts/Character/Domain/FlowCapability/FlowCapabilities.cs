using MageFactory.Flow.Contract;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowCapabilities : IFlowCapabilities {
        private readonly FlowQueries flowQueries;
        private readonly FlowCommandBus flowCommandBus;

        public FlowCapabilities() {
            flowQueries = new FlowQueries();
            flowCommandBus = new FlowCommandBus();
        }

        IFlowQueries IFlowCapabilities.query() {
            return flowQueries;
        }

        IFlowCommandBus IFlowCapabilities.command() {
            return flowCommandBus;
        }
    }
}