using System;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain.CombatCapabilities {
    internal class CombatCommandBus : ICombatCommandBus {
        private readonly CombatContext combatContext;

        internal CombatCommandBus(CombatContext combatContext) {
            this.combatContext = NullGuard.NotNullOrThrow(combatContext);
        }

        public void dispatch(CombatCommand combatCommand) {
            NullGuard.NotNullOrThrow(combatCommand);

            switch (combatCommand) {
                // for now, maybe it should be dispatched by methods instead
                case CreateFlowCombatCommand createFlowCombatCommand:
                    handle(createFlowCombatCommand);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(combatCommand),
                        combatCommand.GetType(),
                        "Unsupported combat command type."
                    );
            }
        }

        private void handle(CreateFlowCombatCommand combatCommand) {
            NullGuard.NotNullOrThrow(combatCommand);

            combatContext.createFlow(combatCommand);
        }
    }
}