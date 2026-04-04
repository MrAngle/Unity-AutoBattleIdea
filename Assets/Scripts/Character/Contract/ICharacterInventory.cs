using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventory : ICombatCharacterInventory {
        //TODO: separate read and command model
        // IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);
        public bool canPlace(PlaceItemQuery placeItemCommand);
        public HashSet<ICharacterEquippedEntryPointToTick> getEntryPointsToTick();

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<ICharacterEquippedItem> neighborItems);
    }
}