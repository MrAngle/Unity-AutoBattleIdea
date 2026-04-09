using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCommandBus {
        // DamageToDeal consumeFlow(ConsumeFlowCommand offensiveFlowCommand, IReadCombatContext combatContext);
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        void takeDamage(DamageToReceive damageToReceive);
    }
}