using MageFactory.CombatContext.Contract;
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
                enemy.apply(flowCommand.damageToDeal);
                return flowCommand.damageToDeal;
            }

            return DamageToDeal.NO_POWER;
        }
    }
}