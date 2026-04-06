using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowQueries : IFlowQueries {
        private ICombatQueries combatQueries;

        public FlowQueries(ICombatQueries combatQueries) {
            this.combatQueries = NullGuard.NotNullOrThrow(combatQueries);
        }

        // public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem) {
        //     combatQueries.try(sourceFlowItem, out IEnumerable<IFlowItem> combatAdjacentFlowItem);
        //     adjacentFlowItem = null;
        //     return false;
        // }
    }
}