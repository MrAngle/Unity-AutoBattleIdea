namespace MageFactory.CombatContext.Api.Event {
    public interface ICombatContextEventPublisher {
        void publish(in CombatCharacterCreatedDtoEvent ev);
    }

    public interface ICombatContextEventRegistry {
        void subscribe(ICombatCharacterCreatedEventListener eventListener);
        void unsubscribe(ICombatCharacterCreatedEventListener eventListener);
    }
}