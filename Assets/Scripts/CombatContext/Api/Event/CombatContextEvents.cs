using MageFactory.Shared.Event;

namespace MageFactory.CombatContext.Api.Event {
    public readonly struct CombatCharacterCreatedDtoEvent : IDomainEvent {
    }

    public interface ICombatCharacterCreatedEventListener
        : IDomainEventListener<CombatCharacterCreatedDtoEvent> {
    }
}