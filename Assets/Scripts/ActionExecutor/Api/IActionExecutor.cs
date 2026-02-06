using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api.Dto;

namespace MageFactory.ActionExecutor.Api {
    public interface IActionExecutor {
        Task executeAsync(ExecuteActionCommand actionCommand);
    }
}