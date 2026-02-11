using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        public CombatCommandBus() {
        }

        public bool post(ICombatCommand command) {
            return false;
        }
    }
}