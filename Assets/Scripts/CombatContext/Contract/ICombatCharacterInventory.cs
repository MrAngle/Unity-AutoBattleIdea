using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterInventory {
        IEnumerable<ICombatCharacterEquippedItem> getPlacedSnapshot();
        ICombatInventory getInventoryGrid();
        bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item);
    }
}