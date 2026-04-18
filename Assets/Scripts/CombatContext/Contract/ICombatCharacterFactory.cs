using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterFactory {
        ICombatCharacterFacade create(CreateCombatCharacterCommand command);
    }
}