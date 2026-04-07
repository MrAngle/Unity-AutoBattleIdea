using System;
using MageFactory.CombatContext.Api;

namespace MageFactory.BattleManager {
    public sealed class BattleSession {
        private readonly BattleRuntime _runtime;
        private readonly ICombatContext _context;

        public BattleSession(BattleRuntime runtime, ICombatContext context) {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void tickOnce() => _runtime.tick(_context);

        public void tickMany(IBattleLoop loop, int steps) {
            if (loop == null) throw new ArgumentNullException(nameof(loop));
            loop.runSteps(steps, tickOnce);
        }

        public ICombatContext context => _context;
    }
}