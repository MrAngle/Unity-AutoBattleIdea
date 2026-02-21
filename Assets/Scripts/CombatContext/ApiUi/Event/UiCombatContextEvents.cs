using MageFactory.Shared.Event;

namespace CombatContext.ApiUi.Event {
    public readonly struct UiCombatCharacterSelected : IUiEvent {
        // public UiCombatCharacterSelected() {
        // }
    }

    public interface IUiCombatCharacterEventListener
        : IUiEventListener<UiCombatCharacterSelected> {
    }
}