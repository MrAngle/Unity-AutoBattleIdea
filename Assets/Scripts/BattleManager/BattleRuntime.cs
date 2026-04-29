using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.BattleManager {
    public class BattleRuntime {
        public void tick(ICombatContext combatContext) {
            IReadOnlyCollection<ICombatCharacterFacade> combatCharacters = combatContext.getAllCharacters();

            foreach (var character in combatCharacters.ToList()) {
                character.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            }
        }
    }
}