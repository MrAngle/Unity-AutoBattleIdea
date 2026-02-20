using System.Collections.Generic;
using MageFactory.Shared.Event.Listener;

namespace MageFactory.Shared.Event {
    public sealed class InventoryEventChannel<TEvent, TListener>
        where TListener : class, IDomainListener<TEvent> {
        private readonly List<TListener> listeners = new();
        private bool subscriberRemoved;

        public void subscribe(TListener listener) {
            if (listener == null) return;

            listeners.Add(listener);
            InventoryEventLogger.logSubscribe<TEvent, TListener>(listener);
        }

        public void unsubscribe(TListener listener) {
            if (listener == null) return;

            for (int i = 0; i < listeners.Count; i++) {
                if (ReferenceEquals(listeners[i], listener)) {
                    listeners[i] = null;
                    subscriberRemoved = true;
                    InventoryEventLogger.logUnsubscribe<TEvent, TListener>(listener);
                }
            }
        }

        public void publish(in TEvent ev) {
            InventoryEventLogger.logPublishStart<TEvent, TListener>(in ev, listeners.Count);

            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                if (listener == null) continue;

                InventoryEventLogger.logPublishToListener<TEvent, TListener>(listener, i);
                listener.onDomainEvent(in ev);
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