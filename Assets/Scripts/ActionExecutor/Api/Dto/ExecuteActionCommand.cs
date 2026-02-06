using System.Threading;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;

namespace MageFactory.ActionExecutor.Api.Dto {
    public record ExecuteActionCommand(
        IItemActionDescription itemActionDescription,
        IEffectContext flowContext,
        CancellationToken cancellationToken = default);
}