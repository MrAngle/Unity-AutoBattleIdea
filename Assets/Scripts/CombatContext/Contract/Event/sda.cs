namespace MageFactory.CombatContext.Contract.Event {
    public interface ICombatContextEvent {
        // TODO
    }

    public interface ICharacterEventHandler {
        void enqueue(ICombatContextEvent evt);
    }
}