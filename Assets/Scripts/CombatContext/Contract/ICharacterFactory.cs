using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterFactory {
        ICombatCharacter create(CreateCombatCharacterCommand command);
    }
}