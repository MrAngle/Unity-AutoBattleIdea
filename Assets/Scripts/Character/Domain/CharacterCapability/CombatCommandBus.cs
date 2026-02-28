using System;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        private CharacterAggregate character;

        public CombatCommandBus(CharacterAggregate character) {
            this.character = character;
        }

        public DamageToDeal consumeFlow(ProcessFlowCommand flowCommand, IReadCombatContext combatContext) {
            if (combatContext.tryGetRandomEnemyOf(character.getId(), out var enemy)) {
                enemy.getCharacterCombatCapabilities().command().apply(flowCommand.damageToDeal);
                return flowCommand.damageToDeal;
            }

            return DamageToDeal.NO_POWER;
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return character.equipItemOrThrow(item);
        }

        public void apply(PowerAmount powerAmount) {
            character.apply(powerAmount);
        }

        public bool tryMoveItemToRight(IFlowItem flowItem) {
            throw new NotImplementedException();
        }
    }
}