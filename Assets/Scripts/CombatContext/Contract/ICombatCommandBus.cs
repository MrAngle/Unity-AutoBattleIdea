using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCommandBus : IFlowCommandBus {
        DamageToDeal consumeFlow(ProcessFlowCommand flowCommand, IReadCombatContext combatContext);
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        void apply(PowerAmount powerAmount);
    }
}