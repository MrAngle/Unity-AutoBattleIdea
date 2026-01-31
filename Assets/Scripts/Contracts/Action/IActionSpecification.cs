using Contracts.Flow;

namespace Contracts.Actionexe {
    public interface IActionSpecification {
        IPreparedAction ToPreparedAction(IFlowContext flowContext);
    }
}