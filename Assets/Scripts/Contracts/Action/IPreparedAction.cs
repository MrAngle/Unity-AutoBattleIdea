namespace Contracts.Actionexe {
    public interface IPreparedAction {
        ActionTiming GetActionTiming();
        void Execute();
    }
}