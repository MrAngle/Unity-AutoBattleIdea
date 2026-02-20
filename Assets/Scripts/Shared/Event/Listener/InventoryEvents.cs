namespace MageFactory.Shared.Event.Listener {
    public interface IDomainListener<TEvent> {
        void onDomainEvent(in TEvent ev);
    }
}