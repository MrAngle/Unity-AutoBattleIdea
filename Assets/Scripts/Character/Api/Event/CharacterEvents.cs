using MageFactory.Character.Api.Event.Dto;
using MageFactory.Shared.Event;

namespace MageFactory.Character.Api.Event {
    public interface IHpChangedEventListener : IDomainEventListener<CharacterHpChangedDtoEvent> {
    }

    public interface ICharacterDeathEventListener : IDomainEventListener<CharacterDeathDtoEvent> {
    }
}