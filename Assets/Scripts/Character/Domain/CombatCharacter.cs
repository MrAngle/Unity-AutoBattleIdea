using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CombatCharacter : ICombatCharacter {
        private readonly CharacterAggregate characterAggregate;
        private readonly ICharacterCombatCapabilities characterCombatCapabilities;

        internal CombatCharacter(CharacterAggregate characterAggregate,
                                 ICharacterCombatCapabilities characterCombatCapabilities) {
            this.characterAggregate = NullGuard.NotNullOrThrow(characterAggregate);
            this.characterCombatCapabilities = NullGuard.NotNullOrThrow(characterCombatCapabilities);
        }

        public Id<CharacterId> getId() {
            return characterAggregate.getId();
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return characterAggregate.equipItemOrThrow(item);
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterAggregate.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return characterAggregate.getInventoryAggregate();
        }

        public long getMaxHp() {
            return characterAggregate.getMaxHp();
        }

        public long getCurrentHp() {
            return characterAggregate.getCurrentHp();
        }

        public void apply(PowerAmount powerAmount) {
            characterAggregate.apply(powerAmount);
        }

        public string getName() {
            return characterAggregate.getName();
        }

        public void cleanup() {
            characterAggregate.cleanup();
        }

        public void combatTick(IFlowConsumer flowConsumer) {
            characterAggregate.combatTick(flowConsumer, characterCombatCapabilities);
        }

        public ICharacterCombatCapabilities getCharacterCombatCapabilities() {
            return characterCombatCapabilities;
        }

        public Team getTeam() {
            return characterAggregate.getTeam();
        }
    }
}