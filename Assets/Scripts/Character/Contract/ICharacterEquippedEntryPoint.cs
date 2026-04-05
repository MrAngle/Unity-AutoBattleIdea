using MageFactory.ActionEffect;
using MageFactory.Shared.Contract;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedEntryPointToTick : IGridItemPlaced {
        IActionDescription prepareItemActionDescription();
    }
}