// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using MageFactory.Inventory.Api.Event;
//
// [assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]
//
// namespace MageFactory.Inventory.Domain.Service {
//     internal sealed class InventoryEventHub : IInventoryEventHub {
//         private readonly List<IInventoryChangedEventListener> inventoryChangedListeners = new();
//         private readonly List<IInventoryItemPlacedEventListener> inventoryItemPlacedEventListener = new();
//
//         private readonly List<InventoryChanged> queueInventoryChanged = new();
//         private readonly List<NewItemPlacedDtoEvent> queueItemPlaced = new();
//
//         private bool subscriberRemoved;
//
//         public void subscribe(IInventoryChangedEventListener inventoryEventListener) {
//             inventoryChangedListeners.Add(inventoryEventListener);
//         }
//
//         public void subscribe(IInventoryItemPlacedEventListener paramInventoryItemPlacedEventListener) {
//             inventoryItemPlacedEventListener.Add(paramInventoryItemPlacedEventListener);
//         }
//
//         public void unsubscribe(IInventoryChangedEventListener listener) {
//             for (int i = 0; i < inventoryChangedListeners.Count; i++) {
//                 if (ReferenceEquals(inventoryChangedListeners[i], listener)) {
//                     inventoryChangedListeners[i] = null;
//                     subscriberRemoved = true;
//                 }
//             }
//         }
//
//
//         public void unsubscribe(IInventoryItemPlacedEventListener listener) {
//             for (int i = 0; i < inventoryItemPlacedEventListener.Count; i++) {
//                 if (ReferenceEquals(inventoryItemPlacedEventListener[i], listener)) {
//                     inventoryItemPlacedEventListener[i] = null;
//                     subscriberRemoved = true;
//                 }
//             }
//         }
//
//         // public void unsubscribe(IInventoryItemPlacedEventListener l)
//         // {
//         //     if (_isPublishing) _pendingRemoveItemPlaced.Add(l);
//         //     else inventoryItemPlacedEventListener.Remove(l);
//         // }
//
//         public void enqueue(in InventoryChanged ev) {
//             queueInventoryChanged.Add(ev);
//         }
//
//         public void enqueue(in NewItemPlacedDtoEvent ev) {
//             queueItemPlaced.Add(ev);
//         }
//
//         public void publishAll() {
//             for (int eventIndex = 0; eventIndex < queueInventoryChanged.Count; eventIndex++) {
//                 var ev = queueInventoryChanged[eventIndex];
//                 for (int listenerIndex = 0; listenerIndex < inventoryChangedListeners.Count; listenerIndex++) {
//                     var listener = inventoryChangedListeners[listenerIndex];
//                     listener?.OnEvent(in ev);
//                 }
//             }
//
//             queueInventoryChanged.Clear();
//
//             for (int e = 0; e < queueItemPlaced.Count; e++) {
//                 var ev = queueItemPlaced[e];
//                 for (int i = 0; i < inventoryItemPlacedEventListener.Count; i++) {
//                     var listener = inventoryItemPlacedEventListener[i];
//                     listener?.OnEvent(in ev);
//                 }
//             }
//
//             queueItemPlaced.Clear();
//
//             compactSubscribers();
//         }
//
//         private void compactSubscribers() {
//             if (subscriberRemoved) {
//                 compactCollection(inventoryChangedListeners);
//                 compactCollection(inventoryItemPlacedEventListener);
//             }
//
//             subscriberRemoved = false;
//         }
//
//         private static void compactCollection<T>(List<T> list) where T : class {
//             int write = 0;
//             for (int read = 0; read < list.Count; read++) {
//                 var v = list[read];
//                 if (v == null) continue;
//                 list[write++] = v;
//             }
//
//             if (write < list.Count)
//                 list.RemoveRange(write, list.Count - write);
//         }
//     }
// }

