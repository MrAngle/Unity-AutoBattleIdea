using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly CombatCharacter combatCharacter;

        internal CombatQueries(CombatCharacter combatCharacter) {
            this.combatCharacter = combatCharacter;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (combatCharacter.getInventoryAggregate()
                .tryGetItemAtCell(cell, out ICharacterEquippedItem combatItem)) {
                item = new CombatCharacterEquippedItem(combatItem);
                return true;
            }

            item = null;
            return false;
        }

        public Team getTeam() {
            return combatCharacter.getTeam();
        }

        public Id<CharacterId> getCharacterId() {
            return combatCharacter.getId();
        }

        public string getCharacterName() {
            return combatCharacter.getName();
        }

        public long getMaxHp() {
            return combatCharacter.getMaxHp();
        }

        public long getCurrentHp() {
            return combatCharacter.getCurrentHp();
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return combatCharacter.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return new CombatCharacterInventory(combatCharacter.getInventoryAggregate());
        }

        // internal bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem,
        //                                        out IEnumerable<ICharacterEquippedItem> characterEquippedItems) {
        //     IEnumerable<GridDirection> gridDirections = new[] { GridDirection.Right }; // for now
        //     if (combatCharacter.getInventoryAggregate().tryGetNeighborItems(
        //             sourceFlowItem,
        //             gridDirections,
        //             out IEnumerable<ICharacterEquippedItem> adjacentItems)) {
        //         characterEquippedItems = adjacentItems;
        //         return true;
        //     }
        //
        //     characterEquippedItems = null;
        //     return false;
        // }

        // private static IEnumerable<IFlowItem> mapToEquippedItems(IEnumerable<ICharacterEquippedItem> placedItems) {
        //     return placedItems.Select(pi => new CombatCharacterEquippedItem(pi));
        // }
    }
}