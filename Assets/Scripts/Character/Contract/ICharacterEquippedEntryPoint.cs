using MageFactory.ActionEffect;
using MageFactory.CombatEvents;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public interface ICharacterEquippedEntryPoint : ICharacterEquippedItem {
        FlowKind getFlowKind();

        EntryPointTriggerKind getTriggerKind();

        ICombatHook getCombatHook();
    }
}