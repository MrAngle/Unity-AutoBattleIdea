using MageFactory.Character.Domain.CombatChar;
using MageFactory.Flow.Contract;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowCapabilities : IFlowCapabilities {
        private readonly FlowQueries flowQueries;
        private readonly FlowCommandBus flowCommandBus;

        public FlowCapabilities(CombatCharacter combatCharacter) {
            flowQueries = new FlowQueries(combatCharacter);
            flowCommandBus = new FlowCommandBus(flowQueries, combatCharacter);
        }

        IFlowQueries IFlowCapabilities.query() {
            return flowQueries;
        }

        IFlowCommandBus IFlowCapabilities.command() {
            return flowCommandBus;
        }
    }
}