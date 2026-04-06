using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CharacterCapability;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowCommandBus : IFlowCommandBus {
        private readonly FlowQueries flowQueries;
        private readonly CharacterCombatCapabilities characterCombatCapabilities;

        public FlowCommandBus(FlowQueries flowQueries, CharacterCombatCapabilities characterCombatCapabilities) {
            this.characterCombatCapabilities = NullGuard.NotNullOrThrow(characterCombatCapabilities);
            this.flowQueries = NullGuard.NotNullOrThrow(flowQueries);
        }

        public bool tryMoveRightAdjacentItemToRight(IFlowItem flowItem) {
            if (characterCombatCapabilities.internalQuery()
                .tryGetRightAdjacentItems(flowItem, out IEnumerable<ICharacterEquippedItem> adjacentItems)) {
                foreach (var item in adjacentItems) {
                    //  return results of move
                    characterCombatCapabilities.internalCommand().tryMoveItemToRight(item);
                }

                return true;
            }

            return false;
        }
    }
}