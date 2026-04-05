using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatInventory {
        int getWidthCellsNumber();
        int getHeightCellsNumber();
        CellState getState(Vector2Int coord);
    }
}