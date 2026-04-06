using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterGridInventory : ICombatInventory {
        private readonly IReadOnlyInventoryGrid readOnlyInventoryGrid;

        public CombatCharacterGridInventory(IReadOnlyInventoryGrid readOnlyInventoryGrid) {
            this.readOnlyInventoryGrid = readOnlyInventoryGrid;
        }

        public int getWidthCellsNumber() {
            return readOnlyInventoryGrid.getWidthCellsNumber();
        }

        public int getHeightCellsNumber() {
            return readOnlyInventoryGrid.getHeightCellsNumber();
        }

        public CellState getState(Vector2Int coord) {
            return readOnlyInventoryGrid.getState(coord);
        }
    }
}