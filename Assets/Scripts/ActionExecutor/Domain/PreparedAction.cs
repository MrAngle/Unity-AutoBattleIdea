using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionExecutor.Domain {
    sealed class PreparedAction {
        private readonly Duration castTime;
        private readonly IReadOnlyList<IOperation> effects;
        private readonly IActionContext actionContext;

        PreparedAction(Duration castTime, IReadOnlyList<IOperation> effects, IActionContext actionContext) {
            this.castTime = castTime;
            this.effects = effects;
            this.actionContext = actionContext;
            // this.actionCommand = actionCommand ?? throw new ArgumentNullException(nameof(actionCommand));
            NullGuard.NotNullCheckOrThrow(this.castTime, this.effects, this.actionContext);
        }

        static PreparedAction from(IActionDescription actionDescription, IActionContext actionContext) {
            // return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
            return new PreparedAction(actionDescription.getCastTime(),
                actionDescription.getEffectsDescriptor().getEffects(),
                actionContext);
        }

        internal static PreparedAction from(ExecuteActionCommand actionCommand) {
            return from(actionCommand.itemActionDescription, actionCommand.flowContext);
        }

        internal Duration getCastTime() {
            return castTime;
        }

        internal void execute() {
            for (var i = 0; i < effects.Count; i++) effects[i].apply(actionContext);
        }
    }
}