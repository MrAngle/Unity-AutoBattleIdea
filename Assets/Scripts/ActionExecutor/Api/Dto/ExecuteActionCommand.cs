using System.Threading;
using MageFactory.ActionEffect;

namespace MageFactory.ActionExecutor.Api.Dto {
    public record ExecuteActionCommand(
        IActionDescription itemActionDescription,
        IActionCapabilities actionCapabilities,
        CancellationToken cancellationToken = default);
}