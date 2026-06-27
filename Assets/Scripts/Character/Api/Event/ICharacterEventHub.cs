using MageFactory.Character.Api.Event.Dto;

namespace MageFactory.Character.Api.Event {
    public interface ICharacterEventPublisher {
        void publish(in CharacterHpChangedDtoEvent ev);
        void publish(in CharacterDeathDtoEvent ev);
        void publish(in CharacterGuardAbsorbedDamageDtoEvent ev);
        void publish(in CharacterStabilityAbsorbedDamageDtoEvent ev);
    }

    public interface ICharacterEventRegistry {
        void subscribe(IHpChangedEventListener eventListener);
        void unsubscribe(IHpChangedEventListener eventListener);

        void subscribe(ICharacterDeathEventListener eventListener);
        void unsubscribe(ICharacterDeathEventListener eventListener);

        void subscribe(IGuardAbsorbedDamageEventListener eventListener);
        void unsubscribe(IGuardAbsorbedDamageEventListener eventListener);

        void subscribe(IStabilityAbsorbedDamageEventListener eventListener);
        void unsubscribe(IStabilityAbsorbedDamageEventListener eventListener);
    }
}