using System;
using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterInventory : ICombatCharacterInventory {
        private readonly ICharacterInventory characterInventory;

        public CombatCharacterInventory(ICharacterInventory characterInventory) {
            this.characterInventory = characterInventory;
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            return characterInventory.getPlacedSnapshot();
        }

        public ICombatInventory getInventoryGrid() {
            return new CombatCharacterGridInventory(characterInventory.getInventoryGrid());
        }

        public bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item) {
            if (characterInventory.tryGetItemAtCell(cell, out ICharacterEquippedItem characterEquippedItem)) {
                item = new CombatCharacterEquippedItem(characterEquippedItem);
                return true;
            }

            item = null;
            return false;
        }

        public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem) {
            throw new NotImplementedException();
        }
    }
}