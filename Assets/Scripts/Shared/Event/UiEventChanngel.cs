namespace MageFactory.Shared.Event {
    namespace MageFactory.Shared.Event {
        public sealed class UiEventChanngel<TEvent, TListener>
            where TEvent : IUiEvent
            where TListener : class, IUiEventListener<TEvent> {
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
}