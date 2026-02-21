using MageFactory.Shared.Event;

namespace MageFactory.CombatContext.Api.Event {
    public readonly struct CombatCharacterCreatedDtoEvent : IDomainEvent {
    }

    public interface ICombatCharacterCreatedEventListener
        : IDomainEventListener<CombatCharacterCreatedDtoEvent> {
    }

    public readonly struct CombatContextCreatedDtoEvent : IDomainEvent {
        public readonly ICombatContext combatContext;

        public CombatContextCreatedDtoEvent(ICombatContext combatContext) {
            this.combatContext = combatContext;
        }
    }

    public interface ICombatContextEventListener
        : IDomainEventListener<CombatContextCreatedDtoEvent> {
    }
}