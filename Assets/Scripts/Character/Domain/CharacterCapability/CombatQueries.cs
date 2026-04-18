using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly CombatCharacter combatCharacter;

        internal CombatQueries(CombatCharacter combatCharacter) {
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