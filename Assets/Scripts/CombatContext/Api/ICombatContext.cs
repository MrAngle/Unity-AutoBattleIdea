using System.Collections.Generic;
using MageFactory.CombatContext.Contract;

namespace MageFactory.CombatContext.Api {
    public interface ICombatContext {
        ICombatCharacter getRandomCharacter(); // TODO remove

        public IReadOnlyCollection<ICombatCharacter> getAllCharacters();
    }
}