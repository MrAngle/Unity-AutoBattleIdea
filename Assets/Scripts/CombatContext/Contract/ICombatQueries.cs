using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries {
        bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item);
    }
}