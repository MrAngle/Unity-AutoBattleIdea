using MageFactory.Character.Api.Event.Dto;

namespace MageFactory.Character.Api.Event {
    public interface ICharacterEventPublisher {
        void publish(in CharacterHpChangedDtoEvent ev);
        void publish(in CharacterDeathDtoEvent ev);
    }

    public interface ICharacterEventRegistry {
        void subscribe(IHpChangedEventListener eventListener);
        void unsubscribe(IHpChangedEventListener eventListener);

        void subscribe(ICharacterDeathEventListener eventListener);
        void unsubscribe(ICharacterDeathEventListener eventListener);
    }
}