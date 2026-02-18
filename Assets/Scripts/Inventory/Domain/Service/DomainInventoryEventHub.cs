using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Inventory.Api.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal sealed class InventoryEventHub : IInventoryEventHub {
        private readonly List<IInventoryChangedEventListener> inventoryChangedListeners = new();
        private readonly List<IInventoryItemPlacedEventListener> inventoryItemPlacedEventListener = new();

        private readonly List<InventoryChanged> queueInventoryChanged = new();
        private readonly List<NewItemPlacedDtoEvent> queueItemPlaced = new();

        public void subscribe(IInventoryChangedEventListener inventoryEventListener) {
            inventoryChangedListeners.Add(inventoryEventListener);
        }

        public void subscribe(IInventoryItemPlacedEventListener paramInventoryItemPlacedEventListener) {
            inventoryItemPlacedEventListener.Add(paramInventoryItemPlacedEventListener);
        }

        public void enqueue(in InventoryChanged ev) {
            queueInventoryChanged.Add(ev);
        }

        public void enqueue(in NewItemPlacedDtoEvent ev) {
            queueItemPlaced.Add(ev);
        }

        public void publishAll() {
            if (queueInventoryChanged.Count > 0) {
                var listenersSnapshot = inventoryChangedListeners.ToArray(); // bezpieczne przy unsubscribe w trakcie
                for (int e = 0; e < queueInventoryChanged.Count; e++) {
                    var ev = queueInventoryChanged[e];
                    for (int i = 0; i < listenersSnapshot.Length; i++)
                        listenersSnapshot[i].OnEvent(in ev);
                }

                queueInventoryChanged.Clear();
            }

            if (queueItemPlaced.Count > 0) {
                var listenersSnapshot = inventoryItemPlacedEventListener.ToArray();
                for (int e = 0; e < queueItemPlaced.Count; e++) {
                    var ev = queueItemPlaced[e];
                    for (int i = 0; i < listenersSnapshot.Length; i++)
                        listenersSnapshot[i].OnEvent(in ev);
                }

                queueItemPlaced.Clear();
            }
        }
    }
}