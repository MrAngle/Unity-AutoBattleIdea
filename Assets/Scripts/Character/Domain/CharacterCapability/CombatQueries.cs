using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly CharacterAggregate characterAggregate;

        internal CombatQueries(CharacterAggregate characterAggregate) {
            this.characterAggregate = characterAggregate;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (characterAggregate.getInventoryAggregate()
                .tryGetItemAtCell(cell, out ICharacterEquippedItem combatItem)) {
                item = new CombatCharacterEquippedItem(combatItem);
                return true;
            }

            item = null;
            return false;
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterAggregate.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return new CombatCharacterInventory(characterAggregate.getInventoryAggregate());
        }

        internal bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem,
                                               out IEnumerable<ICharacterEquippedItem> characterEquippedItems) {
            IEnumerable<GridDirection> gridDirections = new[] { GridDirection.Right };
            if (characterAggregate.getInventoryAggregate().tryGetNeighborItems(
                    sourceFlowItem,
                    gridDirections,
                    out IEnumerable<ICharacterEquippedItem> adjacentItems)) {
                characterEquippedItems = adjacentItems;
                return true;
            }

            characterEquippedItems = null;
            return false;
        }

        private static IEnumerable<IFlowItem> mapToEquippedItems(IEnumerable<ICharacterEquippedItem> placedItems) {
            return placedItems.Select(pi => new CombatCharacterEquippedItem(pi));
        }
    }
}