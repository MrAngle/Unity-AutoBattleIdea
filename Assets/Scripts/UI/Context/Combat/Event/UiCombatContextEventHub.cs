using MageFactory.Shared.Event.MageFactory.Shared.Event;

namespace MageFactory.UI.Context.Combat.Event {
    internal sealed class UiCombatContextEventHub : IUiCombatContextEventPublisher, IUiCombatContextEventRegistry {
        private readonly UiEventChanngel<UiCombatCharacterSelectedEvent, IUiCombatCharacterSelectedEventListener>
            combatCharacterCreatedChannel
                = new();

        public void publish(in UiCombatCharacterSelectedEvent characterSelectedEvent) {
            combatCharacterCreatedChannel.publish(in characterSelectedEvent);
        }

        public void subscribe(IUiCombatCharacterSelectedEventListener selectedEventListener) {
            combatCharacterCreatedChannel.subscribe(selectedEventListener);
        }

        public void unsubscribe(IUiCombatCharacterSelectedEventListener selectedEventListener) {
            combatCharacterCreatedChannel.unsubscribe(selectedEventListener);
        }
    }
}