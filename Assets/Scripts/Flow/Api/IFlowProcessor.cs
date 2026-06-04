using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Api {
    public interface IFlowProcessor {
        void tick(CombatTicks combatTicks);
        bool isFinished();
        Id<ActiveFlowId> getFlowId();
        void collectActiveFlowStates(IActiveFlowStateCollector collector);
    }
}