using System.Collections.Generic;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventory {
        //TODO: separate read and command model
        // IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();

        IEnumerable<IGridItemPlaced> getPlacedSnapshot();

        IInventoryGrid getInventoryGrid();
        bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem item);

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);
        public bool canPlace(PlaceItemQuery placeItemCommand);
        public HashSet<ICharacterEquippedEntryPointToTick> getEntryPointsToTick();

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<ICharacterEquippedItem> neighborItems);
    }
}