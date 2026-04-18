using MageFactory.Character.Domain.CombatChar;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowQueries : IFlowQueries {
        private CombatCharacter combatCharacter;

        public FlowQueries(CombatCharacter combatCharacter) {
            this.combatCharacter = NullGuard.NotNullOrThrow(combatCharacter);
        }

        // public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem) {
        //     combatQueries.try(sourceFlowItem, out IEnumerable<IFlowItem> combatAdjacentFlowItem);
        //     adjacentFlowItem = null;
        //     return false;
        // }
    }
}