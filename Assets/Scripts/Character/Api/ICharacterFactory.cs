using MageFactory.Character.Api.Dto;

namespace MageFactory.Character.Api {
    public interface ICharacterAggregateFactory {
        ICharacter Create(CharacterCreateCommand command);
    }
}