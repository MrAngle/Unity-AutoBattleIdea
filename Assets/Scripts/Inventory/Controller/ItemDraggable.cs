using System.Collections.Generic;
using System.Linq;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Contract;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private ItemDragController _controller;
        private IItemDefinition inventoryPlaceableItem;

        [Inject]
        public void construct() {
        }

        private void Awake() {
            _controller = FindAnyObjectByType<ItemDragController>(FindObjectsInactive.Include);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            inventoryPlaceableItem = getRandomItemDefinition();
            if (inventoryPlaceableItem == null) return;
            _controller?.beginDrag(inventoryPlaceableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            _controller?.updateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            _controller?.endDrag(eventData);
            inventoryPlaceableItem = null;
        }

        private IItemDefinition getRandomItemDefinition() {
            IReadOnlyList<IItemDefinition> allItems =
                ItemDefinition.All
                    .Concat<IItemDefinition>(EntryPointDefinition.All)
                    .ToList();
            return allItems[Random.Range(0, allItems.Count)];
        }
    }
}