using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
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

        public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem) {
            IEnumerable<GridDirection> gridDirections = new[] { GridDirection.Right };
            if (character.getInventoryAggregate().tryGetNeighborItems(
                    sourceFlowItem,
                    gridDirections,
                    out IEnumerable<ICharacterEquippedItem> adjacentItems)) {
                adjacentFlowItem = adjacentItems;
                return true;
            }

            adjacentFlowItem = null;
            return false;
        }
    }
}