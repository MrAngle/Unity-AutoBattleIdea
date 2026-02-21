using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace MageFactory.Shared.Event {
    internal static class MageFactoryEventLogger {
        // W Unity:
        // Edit → Project Settings…
        // Zakładka Player
        // Wybierz platformę (PC, Android, whatever).
        // Sekcja Other Settings → Scripting Define Symbols

        [Conditional("MAGEFACTORY_EVENT_LOG")]
        public static void logSubscribe<TEvent, TListener>(TListener listener) {
            Debug.Log(
                $"[InventoryEvent] SUBSCRIBE " +
                $"event={typeof(TEvent).Name}, listenerType={typeof(TListener).Name}, listener={listener}"
            );
        }

        [Conditional("MAGEFACTORY_EVENT_LOG")]
        public static void logUnsubscribe<TEvent, TListener>(TListener listener) {
            Debug.Log(
                $"[InventoryEvent] UNSUBSCRIBE " +
                $"event={typeof(TEvent).Name}, listenerType={typeof(TListener).Name}, listener={listener}"
            );
        }

        [Conditional("MAGEFACTORY_EVENT_LOG")]
        public static void logPublishStart<TEvent, TListener>(in TEvent ev, int listenerCount) {
            Debug.Log(
                $"[InventoryEvent] PUBLISH " +
                $"event={typeof(TEvent).Name}, listeners={listenerCount}, payload={ev}"
            );
        }

        [Conditional("MAGEFACTORY_EVENT_LOG")]
        public static void logPublishToListener<TEvent, TListener>(TListener listener, int index) {
            Debug.Log(
                $"[InventoryEvent]  -> listener[{index}] " +
                $"type={typeof(TListener).Name}, instance={listener}"
            );
        }
    }
}