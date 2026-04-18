using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowCommandBus : IFlowCommandBus {
        private readonly FlowQueries flowQueries;
        private readonly CombatCharacter combatCharacter;

        public FlowCommandBus(FlowQueries flowQueries, CombatCharacter combatCharacter) {
            this.combatCharacter = NullGuard.NotNullOrThrow(combatCharacter);
            this.flowQueries = NullGuard.NotNullOrThrow(flowQueries);
        }

        public bool tryMoveRightAdjacentItemToRight(IFlowItem sourceFlowItem) {
            IEnumerable<GridDirection> gridDirections = new[] { GridDirection.Right }; // for now
            if (combatCharacter.getInventoryAggregate().tryGetNeighborItems(
                    sourceFlowItem,
                    gridDirections,
                    out IEnumerable<ICharacterEquippedItem> adjacentItems)) {
                foreach (var item in adjacentItems) {
                    //  return results of move
                    combatCharacter.tryMoveItem(item);
                }

                return true;
            }

            return false;
        }
    }
}