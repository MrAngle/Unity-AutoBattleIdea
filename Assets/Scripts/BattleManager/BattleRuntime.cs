using System.Collections.Generic;
using MageFactory.CombatContext.Contract;

namespace MageFactory.BattleManager {
    public class BattleRuntime {
        private readonly List<ICombatCharacter> combatCharacters = new();

        public void register(ICombatCharacter character) {
            combatCharacters.Add(character);
        }

        public void tick() {
            for (int i = 0; i < combatCharacters.Count; i++) {
                combatCharacters[i].combatTick();
            }
            // remember that characters may die during tick
        }
    }
}