using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;

namespace MageFactory.Tests.Unit.TestFixtures {
    public static class BattleSessionTestFixtures {
        public static BattleSession basic(ICombatContext combatContext) {
            return new BattleSession(new BattleRuntime(), combatContext);
        }
    }
}