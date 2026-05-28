using System.Collections.Generic;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.BattleManager {
    public class BattleRuntime {
        public void tick(ICombatContext combatContext) {
            IReadOnlyCollection<ICombatCharacterFacade> combatCharacters = combatContext.getAllCharacters();

            foreach (ICombatCharacterFacade character in combatCharacters) {
                character.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            }
        }
    }
}