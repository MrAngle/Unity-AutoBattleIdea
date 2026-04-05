using MageFactory.ActionEffect;
using MageFactory.Shared.Contract;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedItem : IGridItemPlaced {
        IActionDescription prepareItemActionDescription();
    }
}