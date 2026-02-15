using MageFactory.ActionEffect;
using MageFactory.Shared.Contract;

namespace MageFactory.Flow.Contract {
    public interface IFlowItem : IGridItemPlaced {
        IActionDescription prepareItemActionDescription();
    }
}