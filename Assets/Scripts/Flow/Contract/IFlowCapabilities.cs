namespace MageFactory.Flow.Contract {
    public interface IFlowCapabilities {
        IFlowCommandBus command();
        IFlowQueries query();
    }

    public interface IFlowCommandBus {
        bool tryMoveRightAdjacentItemToRight(IFlowItem flowItem);
    }

    public interface IFlowQueries {
        // bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem);
    }
}