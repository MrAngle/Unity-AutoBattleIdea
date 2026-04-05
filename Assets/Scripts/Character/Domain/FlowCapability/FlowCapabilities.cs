using MageFactory.Character.Domain.CharacterCapability;
using MageFactory.Flow.Contract;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowCapabilities : IFlowCapabilities {
        private readonly FlowQueries flowQueries;
        private readonly FlowCommandBus flowCommandBus;

        public FlowCapabilities(CharacterCombatCapabilities characterCombatCapabilities) {
            flowQueries = new FlowQueries(characterCombatCapabilities.query());
            flowCommandBus = new FlowCommandBus(flowQueries, characterCombatCapabilities);
        }

        IFlowQueries IFlowCapabilities.query() {
            return flowQueries;
        }

        IFlowCommandBus IFlowCapabilities.command() {
            return flowCommandBus;
        }
    }
}