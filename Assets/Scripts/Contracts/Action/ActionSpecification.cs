using Contracts.Flow;
using MageFactory.Shared.Utility;

namespace Contracts.Actionexe {
    public sealed class ActionSpecification : IActionSpecification {
        private readonly IActionDescriptor _actionCommand;
        private readonly ActionTiming _actionTiming;

        public ActionSpecification(ActionTiming actionTiming, IActionDescriptor actionCommand) {
            _actionTiming = actionTiming;
            _actionCommand = actionCommand;
            NullGuard.NotNullCheckOrThrow(_actionTiming, _actionCommand);
        }

        public IPreparedAction ToPreparedAction(IFlowContext flowContext) {
            return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
        }
    }
}