using Contracts.Flow;

namespace Contracts.Actionexe {
    public interface IActionDescriptor {
        IActionCommand ToActionCommand(IFlowContext flowContext);
    }
}