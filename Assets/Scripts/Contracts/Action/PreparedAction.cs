using System;
using Shared.Utility;

namespace Contracts.Actionexe {
    public sealed class PreparedAction : IPreparedAction {
        private readonly IActionCommand _actionCommand;
        private readonly ActionTiming _actionTiming;

        public PreparedAction(ActionTiming timing, IActionCommand command) {
            _actionTiming = timing;
            _actionCommand = command ?? throw new ArgumentNullException(nameof(command));
            NullGuard.NotNullCheckOrThrow(_actionTiming, _actionCommand);
        }

        public ActionTiming GetActionTiming() {
            return _actionTiming;
        }

        public void Execute() {
            _actionCommand.Execute();
        }
    }
}