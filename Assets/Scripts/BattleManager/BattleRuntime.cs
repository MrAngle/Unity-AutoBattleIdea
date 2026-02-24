using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;

namespace MageFactory.BattleManager {
    public class BattleRuntime {
        public void tick(ICombatContext combatContext) {
            IReadOnlyCollection<ICombatCharacter> combatCharacters = combatContext.getAllCharacters();

            foreach (var character in combatCharacters.ToList()) {
                character.combatTick(combatContext.getFlowConsumer());
            }
        }
    }
}