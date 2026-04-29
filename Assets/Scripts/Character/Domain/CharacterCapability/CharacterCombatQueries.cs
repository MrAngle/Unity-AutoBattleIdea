using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CharacterCombatQueries : ICharacterCombatQueries {
        private readonly CombatCharacter combatCharacter;

        internal CharacterCombatQueries(CombatCharacter combatCharacter) {
            this.combatCharacter = combatCharacter;
        }

        public IReadOnlyCombatCharacterData getCharacterInfo() {
            return combatCharacter.getCharacterInfo();
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return combatCharacter.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return new CombatCharacterInventory(combatCharacter.getInventoryAggregate());
        }
    }
}