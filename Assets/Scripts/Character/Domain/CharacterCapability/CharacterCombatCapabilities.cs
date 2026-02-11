using MageFactory.CombatContext.Contract;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CharacterCombatCapabilities : ICharacterCombatCapabilities {
        private readonly ICombatCharacter character;
        private readonly CombatCommandBus combatCommandBus;
        private readonly CombatQueries combatQueries;

        internal CharacterCombatCapabilities(ICombatCharacter character) {
            this.character = character;
            combatCommandBus = new CombatCommandBus();
            combatQueries = new CombatQueries(this.character);
        }

        public ICombatCommandBus command() {
            return combatCommandBus;
        }

        public ICombatQueries query() {
            return combatQueries;
        }
    }
}