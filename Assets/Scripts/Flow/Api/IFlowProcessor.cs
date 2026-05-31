using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Api {
    public interface IFlowProcessor {
        void tick(CombatTicks combatTicks);
        bool isFinished();
        void collectActiveFlowCastStates(IActiveFlowCastStateCollector collector);
    }
}