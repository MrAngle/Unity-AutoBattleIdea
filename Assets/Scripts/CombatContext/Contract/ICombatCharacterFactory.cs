using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterFactory {
        ICharacterCombatCapabilities create(CreateCombatCharacterCommand command);
    }
}