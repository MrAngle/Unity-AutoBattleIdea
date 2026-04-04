using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionExecutor.Domain {
    internal sealed class PreparedAction {
        private readonly Duration castTime;
        private readonly IReadOnlyList<IOperation> effects;
        private readonly IActionCapabilities actionCapabilities;

        PreparedAction(Duration castTime, IReadOnlyList<IOperation> effects, IActionCapabilities actionCapabilities) {
            this.castTime = castTime;
            this.effects = effects;
            this.actionCapabilities = actionCapabilities;
            NullGuard.NotNullCheckOrThrow(this.castTime, this.effects, this.actionCapabilities);
        }

        static PreparedAction from(IActionDescription actionDescription, IActionCapabilities actionCapabilities) {
            return new PreparedAction(actionDescription.getCastTime(),
                actionDescription.getEffectsDescriptor().getEffects(),
                actionCapabilities);
        }

        internal static PreparedAction from(ExecuteActionCommand actionCommand) {
            return from(actionCommand.itemActionDescription, actionCommand.actionCapabilities);
        }

        internal Duration getCastTime() {
            return castTime;
        }

        internal void execute() {
            for (var i = 0; i < effects.Count; i++) effects[i].apply(actionCapabilities);
        }
    }
}