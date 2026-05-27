using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain.CombatCapabilities {
    internal class CombatQueries : ICombatQueries {
        private readonly IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters;

        internal CombatQueries(IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters) {
            this.characters = NullGuard.NotNullOrThrow(characters);
        }

        public int getActiveFlowCount() {
            var activeFlowCount = 0;

            foreach (ICombatCharacterFacade character in characters.Values) {
                activeFlowCount += character.query().getActiveFlowCount();
            }

            return activeFlowCount;
        }

        public int getActiveFlowCountForCharacter(Id<CharacterId> characterId) {
            NullGuard.ValidIdOrThrow(characterId);

            return characters.TryGetValue(characterId, out ICombatCharacterFacade character)
                ? character.query().getActiveFlowCount()
                : 0;
        }

        public int getCreatedFlowCount() {
            var createdFlowCount = 0;

            foreach (ICombatCharacterFacade character in characters.Values) {
                createdFlowCount += character.query().getCreatedFlowsInCurrentBattleCount();
            }

            return createdFlowCount;
        }

        public int getCreatedFlowCountForCharacter(Id<CharacterId> characterId) {
            NullGuard.ValidIdOrThrow(characterId);

            return characters.TryGetValue(characterId, out ICombatCharacterFacade character)
                ? character.query().getCreatedFlowsInCurrentBattleCount()
                : 0;
        }
    }
}