using MageFactory.Character.Api.Dto;

namespace MageFactory.Character.Api {
    public interface ICharacterFactory {
        ICharacter create(CharacterCreateCommand command);
    }
}