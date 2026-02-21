namespace MageFactory.CombatContext.Api.Event {
    public interface ICombatContextEventPublisher {
        void publish(in CombatCharacterCreatedDtoEvent ev);
        void publish(in CombatContextCreatedDtoEvent ev);
    }

    public interface ICombatContextEventRegistry {
        void subscribe(ICombatCharacterCreatedEventListener eventListener);
        void unsubscribe(ICombatCharacterCreatedEventListener eventListener);

        void subscribe(ICombatContextEventListener eventListener);
        void unsubscribe(ICombatContextEventListener eventListener);
    }
}