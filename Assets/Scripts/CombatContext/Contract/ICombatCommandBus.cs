using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCommandBus {
        DamageToDeal consumeFlow(ProcessFlowCommand flowCommand, IReadCombatContext combatContext);
    }
}