using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CharacterCombatCapabilities : ICharacterCombatCapabilities {
        private readonly CombatCommandBus combatCommandBus;
        private readonly CombatQueries combatQueries;

        internal CharacterCombatCapabilities(CombatCharacter characterAggregate) {
            combatCommandBus = new CombatCommandBus(characterAggregate);
            combatQueries = new CombatQueries(characterAggregate);
        }

        public ICombatCommandBus command() {
            return combatCommandBus;
        }

        public ICombatQueries query() {
            return combatQueries;
        }

        internal CombatCommandBus internalCommand() {
            return combatCommandBus;
        }

        internal CombatQueries internalQuery() {
            return combatQueries;
        }
    }
}