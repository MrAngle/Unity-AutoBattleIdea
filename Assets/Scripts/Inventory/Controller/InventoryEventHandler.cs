// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using MageFactory.Context;
// using MageFactory.Inventory.Api.Event;
// using MageFactory.Shared.Utility;
// using UnityEngine;
// using Zenject;
//
// [assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]
// namespace MageFactory.Inventory.Controller {
//     public sealed class InventoryEventHandler : MonoBehaviour, IInventoryEventHandler {
//         private readonly Queue<IInventoryEvent> queue = new();
//
//         private InventoryGridView gridView;
//
//         [Inject] private InventoryPanelPrefabInitializer initializer;
//
//         private void Awake() {
//             gridView = initializer.CreateGridView(transform);
//         }
//
//         public void Enqueue(IInventoryEvent evt) => queue.Enqueue(evt);
//
//         private void LateUpdate() {
//             while (queue.Count > 0) Apply(queue.Dequeue());
//         }
//
//         private void Apply(IInventoryEvent evt) {
//             // operacje na gridView
//         }
//     }
//
// }

