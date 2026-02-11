using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterInventory {
        IEnumerable<ICombatCharacterEquippedItem> getPlacedSnapshot();

        // IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();
        ICombatInventory getInventoryGrid();

        // public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);
        // public bool canPlace(PlaceItemQuery placeItemCommand);
        bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item);
    }
}