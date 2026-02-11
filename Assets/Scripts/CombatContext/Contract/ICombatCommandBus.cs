using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCommandBus {
        bool post(ICombatCommand command);
    }
}