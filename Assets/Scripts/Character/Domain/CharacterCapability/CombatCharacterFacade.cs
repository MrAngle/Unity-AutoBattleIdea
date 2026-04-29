using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCharacterFacade : ICombatCharacterFacade {
        private readonly CharacterCombatCommandBus characterCombatCommandBus;
        private readonly CharacterCombatQueries characterCombatQueries;

        internal CombatCharacterFacade(CombatCharacter characterAggregate) {
            characterCombatCommandBus = new CharacterCombatCommandBus(characterAggregate);
            characterCombatQueries = new CharacterCombatQueries(characterAggregate);
        }

        public ICharacterCombatCommandBus command() {
            return characterCombatCommandBus;
        }

        public ICharacterCombatQueries query() {
            return characterCombatQueries;
        }
    }
}