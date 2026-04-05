using System.Collections.Generic;

namespace MageFactory.Flow.Contract {
    public interface IFlowCapabilities {
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