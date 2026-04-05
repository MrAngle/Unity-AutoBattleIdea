using System;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        private CharacterAggregate characterAggregate;

        public CombatCommandBus(CharacterAggregate characterAggregate) {
            this.characterAggregate = characterAggregate;
        }

        public DamageToDeal consumeFlow(ProcessFlowCommand flowCommand, IReadCombatContext combatContext) {
            if (combatContext.tryGetRandomEnemyOf(characterAggregate.getId(), out var enemy)) {
                enemy.getCharacterCombatCapabilities().command().apply(flowCommand.damageToDeal);
                return flowCommand.damageToDeal;
            }

            return DamageToDeal.NO_POWER;
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return new CombatCharacterEquippedItem(characterAggregate.equipItemOrThrow(item));
        }

        public void apply(PowerAmount powerAmount) {
            characterAggregate.apply(powerAmount);
        }

        public bool tryMoveItemToRight(IFlowItem flowItem) {
            throw new NotImplementedException();
        }
    }
}