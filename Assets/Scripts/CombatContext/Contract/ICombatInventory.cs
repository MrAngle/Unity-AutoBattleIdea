using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatInventory {
        int Width { get; }
        int Height { get; }
        CellState getState(Vector2Int coord);
    }
}