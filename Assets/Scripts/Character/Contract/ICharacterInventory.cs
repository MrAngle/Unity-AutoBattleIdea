using System.Collections.Generic;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventory {
        //TODO: separate read and command model
        // IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();

        IEnumerable<IGridItemPlaced> getPlacedSnapshot();

        IReadOnlyInventoryGrid getInventoryGrid();
        bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem item);
        bool tryGetItemById(Id<ItemId> itemId, out ICharacterEquippedItem item);

        bool tryGetEntryPointById(Id<ItemId> itemId, out ICharacterEquippedEntryPoint entryPoint);

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);
        public bool canPlace(PlaceItemQuery placeItemCommand);
        public IReadOnlyCollection<ICharacterEquippedEntryPoint> getEntryPoints();

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<ICharacterEquippedItem> neighborItems);

        bool tryMoveItem(ICharacterEquippedItem itemToMove, Vector2Int newPosition);

        public IReadOnlyCollection<CharacterCombatTickableItemAction> getTickableItems();
    }
}