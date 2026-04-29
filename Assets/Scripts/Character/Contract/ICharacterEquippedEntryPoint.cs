using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedEntryPointToTick : ICharacterEquippedItem {
        FlowKind getFlowKind();
    }
}