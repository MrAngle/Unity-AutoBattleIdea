using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api.Dto;

namespace MageFactory.ActionExecutor.Api {
    public interface IActionExecutor {
        // Task ExecuteAsync(IPreparedAction preparedAction, CancellationToken cancellationToken = default);
        Task executeAsync(ExecuteActionCommand actionCommand);
    }
}