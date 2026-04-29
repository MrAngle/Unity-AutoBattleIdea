using MageFactory.CombatContext.Domain.CombatCapabilities.MageFactory.CombatContext.Domain;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain.CombatCapabilities {
    internal class CombatCapabilities : ICombatCapabilities {
        private readonly CombatCommandBus combatCommandBus;
        private readonly CombatQueries combatQueries;

        internal CombatCapabilities(CombatCommandBus combatCommandBus, CombatQueries combatQueries) {
            this.combatCommandBus = NullGuard.NotNullOrThrow(combatCommandBus);
            this.combatQueries = NullGuard.NotNullOrThrow(combatQueries);
        }

        public ICombatCommandBus command() {
            return combatCommandBus;
        }

        public ICombatQueries query() {
            return combatQueries;
        }
    }
}