using System.Collections.Generic;
using MageFactory.Shared.Contract;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterInventory {
        IEnumerable<IGridItemPlaced> getPlacedSnapshot();
        ICombatInventory getInventoryGrid();
    }
}