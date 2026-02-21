using MageFactory.CombatContext.Api.Event;

namespace CombatContext.ApiUi.Event {
    public interface IUiCombatContextEventHub {
        void publish(in CombatCharacterCreatedDtoEvent ev);
    }

    public interface ICombatContextEventRegistry {
        void subscribe(ICombatCharacterCreatedEventListener eventListener);
        void unsubscribe(ICombatCharacterCreatedEventListener eventListener);
    }
}