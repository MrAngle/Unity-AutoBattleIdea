using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedEntryPoint : ICharacterEquippedItem {
        FlowKind getFlowKind();
    }
}