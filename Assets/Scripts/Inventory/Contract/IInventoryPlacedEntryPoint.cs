using MageFactory.Character.Contract;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedEntryPoint : IInventoryPlacedItem, ICharacterEquippedEntryPointToTick {
    }
}