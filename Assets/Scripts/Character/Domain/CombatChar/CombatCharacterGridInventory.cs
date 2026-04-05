using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterGridInventory : ICombatInventory {
        private readonly IInventoryGrid inventoryGrid;

        public CombatCharacterGridInventory(IInventoryGrid inventoryGrid) {
            this.inventoryGrid = inventoryGrid;
        }

        public int getWidthCellsNumber() {
            return inventoryGrid.getWidthCellsNumber();
        }

        public int getHeightCellsNumber() {
            return inventoryGrid.getHeightCellsNumber();
        }

        public CellState getState(Vector2Int coord) {
            return inventoryGrid.getState(coord);
        }
    }
}