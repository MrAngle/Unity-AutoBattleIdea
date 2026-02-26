using MageFactory.Shared.Event;
using MageFactory.Shared.Id;

namespace MageFactory.Character.Api.Event.Dto {
    public readonly struct HpChangedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long newHp;
        public readonly long previousHpValue;

        public HpChangedDtoEvent(Id<CharacterId> characterId,
                                 long newHp,
                                 long previousHpValue) {
            this.characterId = characterId;
            this.newHp = newHp;
            this.previousHpValue = previousHpValue;
        }
    }

    public readonly struct CharacterDeathDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;

        public CharacterDeathDtoEvent(Id<CharacterId> characterId) {
            this.characterId = characterId;
        }
    }
}