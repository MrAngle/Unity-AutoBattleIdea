using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using TimeSystem;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.ActionExecutor.Domain {
    internal sealed class ActionExecutorService : IActionExecutor {
        public async Task executeAsync(
            ExecuteActionCommand actionCommand /*, CancellationToken cancellationToken = default*/) {
            PreparedAction preparedAction = PreparedAction.from(actionCommand);

            var duration = preparedAction.getCastTime().getValue();

            await TimeModule.ContinueIn(duration, actionCommand.cancellationToken);
            preparedAction.execute();
        }
    }
}