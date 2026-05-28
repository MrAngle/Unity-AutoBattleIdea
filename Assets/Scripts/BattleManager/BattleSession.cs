using System;
using MageFactory.CombatContext.Api;

namespace MageFactory.BattleManager {
    public sealed class BattleSession {
        private readonly BattleRuntime runtime;
        private readonly ICombatContext context;
        private readonly BattleSessionSettings settings;

        public BattleSession(
            BattleRuntime runtime,
            ICombatContext context,
            BattleSessionSettings settings
        ) {
            this.runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void tickOnce() {
            runtime.tick(context);
        }

        public void tickMany(IBattleLoop loop, int steps) {
            if (loop == null) throw new ArgumentNullException(nameof(loop));
            loop.runSteps(steps, tickOnce);
        }

        public ICombatContext getContext() {
            return context;
        }

        public BattleSessionSettings getSettings() {
            return settings;
        }
    }
}