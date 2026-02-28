using System;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using UnityEngine;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly CharacterAggregate character;

        internal CombatQueries(CharacterAggregate character) {
            this.character = character;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (!character.getInventoryAggregate()
                    .tryGetItemAtCell(cell, out ICombatCharacterEquippedItem combatItem)) {
                item = null;
                return false;
            }

            item = combatItem;
            return true;
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return character.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return character.getInventoryAggregate();
        }

        public bool tryGetRightAdjacentItem(IFlowItem sourceFlowItem, out IFlowItem adjacentFlowItem) {
            // character.getInventoryAggregate().tryGetNeighborItems(sourceFlowItem, out adjacentFlowItem);
            throw new NotImplementedException();
        }
    }
}