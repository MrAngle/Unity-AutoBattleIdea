using MageFactory.CombatContext.Contract;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CharacterCombatCapabilities : ICharacterCombatCapabilities {
        private readonly CombatCommandBus combatCommandBus;
        private readonly CombatQueries combatQueries;

        internal CharacterCombatCapabilities(CharacterAggregate character) {
            combatCommandBus = new CombatCommandBus(character);
            combatQueries = new CombatQueries(character);
        }

        public ICombatCommandBus command() {
            return combatCommandBus;
        }

        public ICombatQueries query() {
            return combatQueries;
        }
    }
}