using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Inventory.Domain {
    // odpowiednik
    // public sealed class ActionSpecification : IActionSpecification {
    //     private readonly IActionDescriptor _actionCommand;
    //     private readonly ActionTiming _actionTiming;
    //
    //     public ActionSpecification(ActionTiming actionTiming, IActionDescriptor actionCommand) {
    //         _actionTiming = actionTiming;
    //         _actionCommand = actionCommand;
    //         NullGuard.NotNullCheckOrThrow(_actionTiming, _actionCommand);
    //     }
    //
    //     public IPreparedAction ToPreparedAction(IFlowContext flowContext) {
    //         return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
    //     }
    // }

    public sealed class ItemActionDescription : IItemActionDescription {
        private readonly Duration castTime;
        private readonly IEffectsDescriptor effectsDescriptor;

        public ItemActionDescription(Duration castTime, IEffectsDescriptor effectsDescriptor) {
            this.castTime = castTime;
            this.effectsDescriptor = effectsDescriptor;
            NullGuard.NotNullCheckOrThrow(this.castTime, this.effectsDescriptor);
        }

        public Duration getCastTime() {
            return castTime;
        }

        public IEffectsDescriptor getEffectsDescriptor() {
            return effectsDescriptor;
        }

        // public IPreparedAction ToPreparedAction(IFlowContext flowContext) {
        //     return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
        // }
    }
}