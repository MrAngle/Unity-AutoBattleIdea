using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Api {
    public interface ICombatContext {
        ICombatCharacter getRandomCharacter(); // TODO remove

        public IReadOnlyCollection<ICombatCharacter> getAllCharacters();
        public ICombatCharacter getCombatCharacterById(Id<CharacterId> id);
    }
}