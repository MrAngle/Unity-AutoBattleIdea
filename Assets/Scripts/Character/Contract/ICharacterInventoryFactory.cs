using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterInventoryFactory {
        ICharacterInventory createCharacterInventory(GridDimensions gridDimensions);
    }
}