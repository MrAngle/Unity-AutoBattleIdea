using System.Collections.Generic;

namespace MageFactory.Shared.Event {
    public sealed class MageFactoryEventChannel<TEvent, TListener>
        where TEvent : IMageFactoryEvent
        where TListener : class, IMageEventListener<TEvent> {
        private readonly List<TListener> listeners = new();
        private bool subscriberRemoved;

        public void subscribe(TListener listener) {
            if (listener == null) return;
            if (listeners.Contains(listener)) return;

            listeners.Add(listener);
            MageFactoryEventLogger.logSubscribe<TEvent, TListener>(listener);
        }

        public void unsubscribe(TListener listener) {
            if (listener == null) return;

            for (int i = 0; i < listeners.Count; i++) {
                if (ReferenceEquals(listeners[i], listener)) {
                    listeners[i] = null;
                    subscriberRemoved = true;
                    MageFactoryEventLogger.logUnsubscribe<TEvent, TListener>(listener);
                }
            }
        }

        public void publish(in TEvent ev) {
            MageFactoryEventLogger.logPublishStart<TEvent, TListener>(in ev, listeners.Count);

            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                if (listener == null) continue;

                MageFactoryEventLogger.logPublishToListener<TEvent, TListener>(listener, i);
                listener.onEvent(in ev);
            }

            compactSubscribersIfNeeded();
        }

        private void compactSubscribersIfNeeded() {
            if (!subscriberRemoved) return;

            int write = 0;
            for (int read = 0; read < listeners.Count; read++) {
                var v = listeners[read];
                if (v == null) continue;
                listeners[write++] = v;
            }

            if (write < listeners.Count) {
                listeners.RemoveRange(write, listeners.Count - write);
            }

            subscriberRemoved = false;
        }
    }
}