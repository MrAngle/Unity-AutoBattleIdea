using System.Collections.Generic;
using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Api {
    public interface ICombatContextFactory {
        ICombatContext create(IReadOnlyList<CreateCombatCharacterCommand> createCombatCharacterCommands);
    }
}