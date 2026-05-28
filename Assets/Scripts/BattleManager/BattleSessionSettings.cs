using System;

namespace MageFactory.BattleManager {
    public sealed class BattleSessionSettings {
        private const int DefaultCombatTicksPerRealSecond = 10;

        private readonly int combatTicksPerRealSecond;

        public BattleSessionSettings(int combatTicksPerRealSecond) {
            if (combatTicksPerRealSecond < 1) {
                throw new ArgumentOutOfRangeException(
                    nameof(combatTicksPerRealSecond),
                    "Battle session must execute at least one combat tick per real second.");
            }

            this.combatTicksPerRealSecond = combatTicksPerRealSecond;
        }

        public static BattleSessionSettings createDefault() {
            return new BattleSessionSettings(DefaultCombatTicksPerRealSecond);
        }

        public static int getDefaultCombatTicksPerRealSecond() {
            return DefaultCombatTicksPerRealSecond;
        }

        public int getCombatTicksPerRealSecond() {
            return combatTicksPerRealSecond;
        }

        public float getRealSecondsPerCombatTick() {
            return 1f / combatTicksPerRealSecond;
        }
    }
}