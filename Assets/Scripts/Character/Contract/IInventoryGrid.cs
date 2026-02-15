using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface IInventoryGrid : ICombatInventory {
        bool canPlace(ShapeArchetype data, Vector2Int origin);
        void place(ShapeArchetype data, Vector2Int origin);
    }
}