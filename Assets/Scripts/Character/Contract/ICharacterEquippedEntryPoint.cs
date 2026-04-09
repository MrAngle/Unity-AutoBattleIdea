using MageFactory.ActionEffect;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedEntryPointToTick : IGridItemPlaced {
        IActionDescription prepareItemActionDescription();

        FlowKind getFlowKind();
    }
}