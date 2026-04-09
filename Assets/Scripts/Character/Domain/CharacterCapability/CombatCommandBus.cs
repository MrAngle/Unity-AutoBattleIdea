using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        private CharacterAggregate characterAggregate;

        public CombatCommandBus(CharacterAggregate characterAggregate) {
            this.characterAggregate = characterAggregate;
        }

        // public DamageToDeal consumeFlow(ConsumeFlowCommand offensiveFlowCommand, IReadCombatContext combatContext) {
        //     if (combatContext.tryGetRandomEnemyOf(characterAggregate.getId(), out ICombatCharacter enemy)) {
        //         enemy.getCharacterCombatCapabilities().command().takeDamage(offensiveFlowCommand.damageToDeal);
        //         return offensiveFlowCommand.damageToDeal;
        //     }
        //
        //     return DamageToDeal.NO_POWER;
        // }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return new CombatCharacterEquippedItem(characterAggregate.equipItemOrThrow(item));
        }

        public void takeDamage(DamageToReceive powerAmount) {
            characterAggregate.takeDamage(powerAmount);
            // characterAggregate.apply(powerAmount);
        }

        internal bool tryMoveItemToRight(ICharacterEquippedItem characterEquippedItem) {
            return characterAggregate.tryMoveItem(characterEquippedItem);
        }
    }
}