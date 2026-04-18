using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Api {
    public interface ICombatContext {
        ICombatCharacterFacade getRandomCharacter(); // TODO remove

        public IReadOnlyCollection<ICombatCharacterFacade> getAllCharacters();
        public ICombatCharacterFacade getCombatCharacterById(Id<CharacterId> id);
        public IFlowConsumer getFlowConsumer();
    }
}