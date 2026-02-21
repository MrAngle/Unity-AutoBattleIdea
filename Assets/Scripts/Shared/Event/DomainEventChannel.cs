namespace MageFactory.Shared.Event {
    public sealed class DomainEventChannel<TEvent, TListener>
        where TEvent : IDomainEvent
        where TListener : class, IDomainEventListener<TEvent> {
        private readonly MageFactoryEventChannel<TEvent, TListener> mageFactoryEventChannel = new();

        public void subscribe(TListener listener) {
            mageFactoryEventChannel.subscribe(listener);
        }

        public void unsubscribe(TListener listener) {
            mageFactoryEventChannel.unsubscribe(listener);
        }

        public void publish(in TEvent ev) {
            mageFactoryEventChannel.publish(in ev);
        }
    }
}