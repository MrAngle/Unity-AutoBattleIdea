using MageFactory.CombatContext.Contract;
using MageFactory.FlowRouting;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowAggregateFacade create(ICombatCharacterEquippedItem startNode, IFlowRouter router);
    }
}