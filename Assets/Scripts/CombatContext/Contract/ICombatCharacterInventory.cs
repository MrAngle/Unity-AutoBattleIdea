using System.Collections.Generic;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterInventory {
        IEnumerable<IGridItemPlaced> getPlacedSnapshot();
        ICombatInventory getInventoryGrid();
        bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item);
        public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem);
    }
}