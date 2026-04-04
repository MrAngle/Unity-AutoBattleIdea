using System.Collections.Generic;
using MageFactory.ActionEffect;

namespace MageFactory.Flow.Contract {
    public interface IFlowCapabilities : IActionCapabilities {
        IFlowCommandBus command();
        IFlowQueries query();
    }

    public interface IFlowCommandBus {
        bool tryMoveItemToRight(IFlowItem flowItem);
    }

    public interface IFlowQueries {
        bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem);
    }
}