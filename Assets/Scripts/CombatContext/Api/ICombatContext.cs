using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Api {
    public interface ICombatContext {
        ICharacterCombatCapabilities getRandomCharacter(); // TODO remove

        public IReadOnlyCollection<ICharacterCombatCapabilities> getAllCharacters();
        public ICharacterCombatCapabilities getCombatCharacterById(Id<CharacterId> id);
        public IFlowConsumer getFlowConsumer();
    }
}