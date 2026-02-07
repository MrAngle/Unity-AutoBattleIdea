using System.Threading;
using MageFactory.ActionEffect;

namespace MageFactory.ActionExecutor.Api.Dto {
    public record ExecuteActionCommand(
        IActionDescription itemActionDescription,
        IActionContext flowContext,
        CancellationToken cancellationToken = default);
}