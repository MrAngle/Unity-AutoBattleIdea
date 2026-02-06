using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using TimeSystem;

namespace MageFactory.ActionExecutor.Domain {
    public sealed class ActionExecutor : IActionExecutor {
        // public async Task ExecuteAsync(IPreparedAction preparedAction, CancellationToken cancellationToken = default) {
        //     // var spd = Mathf.Max(0.001f, speedMultiplier);
        //     // var duration = action.BaseDurationSeconds / spd;
        //     var duration = preparedAction.GetActionTiming().DurationSeconds();
        //
        //     await TimeModule.ContinueIn(duration, cancellationToken);
        //     preparedAction.Execute();
        // }

        public async Task executeAsync(
            ExecuteActionCommand actionCommand /*, CancellationToken cancellationToken = default*/) {
            PreparedAction preparedAction = PreparedAction.from(actionCommand);

            var duration = preparedAction.getCastTime().getValue();

            await TimeModule.ContinueIn(duration, actionCommand.cancellationToken);
            preparedAction.execute();
        }
    }
}