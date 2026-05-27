using MageFactory.Shared.Model;

namespace MageFactory.Flow.Api {
    public interface IFlowProcessor {
        void tick(CombatTicks combatTicks);
        bool isFinished();
    }
}