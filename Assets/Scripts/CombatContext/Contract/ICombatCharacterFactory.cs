using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterFactory {
        ICombatCharacter create(CreateCombatCharacterCommand command);
    }
}