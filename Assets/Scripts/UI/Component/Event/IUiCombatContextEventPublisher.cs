namespace MageFactory.UI.Context.Combat.Event {
    public interface IUiCombatContextEventPublisher {
        void publish(in UiCombatCharacterSelectedEvent ev);
    }

    public interface IUiCombatContextEventRegistry {
        void subscribe(IUiCombatCharacterSelectedEventListener selectedEventListener);
        void unsubscribe(IUiCombatCharacterSelectedEventListener selectedEventListener);
    }
}