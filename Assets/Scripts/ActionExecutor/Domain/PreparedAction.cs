using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionExecutor.Domain {
    public sealed class PreparedAction /*: IPreparedAction*/ {
        // private readonly ActionCommand actionCommand;
        private readonly Duration castTime;
        private readonly IReadOnlyList<IEffect> effects;
        private readonly IEffectContext effectContext;

        // public PreparedAction(Duration castTime, ActionCommand actionCommand) {
        //     this.castTime = castTime;
        //     this.actionCommand = actionCommand ?? throw new ArgumentNullException(nameof(actionCommand));
        //     NullGuard.NotNullCheckOrThrow(this.castTime, this.actionCommand);
        // }        

        public PreparedAction(Duration castTime, IReadOnlyList<IEffect> effects, IEffectContext effectContext) {
            this.castTime = castTime;
            this.effects = effects;
            this.effectContext = effectContext;
            // this.actionCommand = actionCommand ?? throw new ArgumentNullException(nameof(actionCommand));
            NullGuard.NotNullCheckOrThrow(this.castTime, this.effects, this.effectContext);
        }

        public static PreparedAction from(IItemActionDescription itemActionDescription, IEffectContext effectContext) {
            // return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
            return new PreparedAction(itemActionDescription.getCastTime(),
                itemActionDescription.getEffectsDescriptor().getEffects(),
                effectContext);
        }

        public static PreparedAction from(ExecuteActionCommand actionCommand) {
            return from(actionCommand.itemActionDescription, actionCommand.flowContext);
        }

        public Duration getCastTime() {
            return castTime;
        }

        // public void execute() {
        //     _actionCommand.Execute();
        // }

        public void execute() {
            for (var i = 0; i < effects.Count; i++) effects[i].apply(effectContext);
        }

        // public IPreparedAction ToPreparedAction(IFlowContext flowContext) {
        //     return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
        // }

        // public ActionTiming GetActionTiming() {
        //     return _actionTiming;
        // }
        //
        // public void Execute() {
        //     _actionCommand.Execute();
        // }
    }
}