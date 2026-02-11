using System.Collections.Generic;
using MageFactory.CombatContext.Contract;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventory : /*IInventoryReadModel, */ICombatCharacterInventory {
        //TODO: separate read and command model
        IEnumerable<ICharacterEquippedItem> getPlacedSnapshot();

        // IInventoryGrid getInventoryGrid();
        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand);
        public bool canPlace(PlaceItemQuery placeItemCommand);
    }
}