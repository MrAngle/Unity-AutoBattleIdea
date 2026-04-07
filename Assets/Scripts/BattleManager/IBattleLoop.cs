using System;

namespace MageFactory.BattleManager {
    public interface IBattleLoop {
        void runSteps(int steps, Action step);
    }

    public sealed class ManualBattleLoop : IBattleLoop {
        public void runSteps(int steps, Action step) {
            if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps));
            if (step == null) throw new ArgumentNullException(nameof(step));

            for (var i = 0; i < steps; i++) step();
        }
    }
}