namespace MageFactory.Flow.Contract {
    public interface IFlowCapabilities {
        IFlowCommandBus command();
        IFlowQueries query();
    }

    public interface IFlowCommandBus {
        bool tryMoveItemToRight(IFlowItem flowItem);
    }

    public interface IFlowQueries {
        bool tryGetRightAdjacentItem(IFlowItem sourceFlowItem, out IFlowItem adjacentFlowItem);
    }
}