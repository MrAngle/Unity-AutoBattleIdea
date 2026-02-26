using MageFactory.Shared.Event;
using MageFactory.Shared.Id;

namespace MageFactory.UI.Context.Combat.Event {
    public readonly struct UiCombatCharacterSelectedEvent : IUiEvent {
        public readonly Id<CharacterId> characterId;

        public UiCombatCharacterSelectedEvent(Id<CharacterId> characterId) {
            this.characterId = characterId;
        }
    }

    public interface IUiCombatCharacterSelectedEventListener
        : IUiEventListener<UiCombatCharacterSelectedEvent> {
    }
}