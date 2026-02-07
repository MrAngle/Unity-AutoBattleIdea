using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface IGridInspector {
        bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem item);
    }
}