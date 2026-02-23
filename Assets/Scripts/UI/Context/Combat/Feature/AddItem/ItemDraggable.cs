using System.Collections.Generic;
using System.Linq;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Contract;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private ItemDragService service;
        private IItemDefinition inventoryPlaceableItem;

        [Inject]
        public void construct(ItemDragService service) {
            this.service = service;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            inventoryPlaceableItem = getRandomItemDefinition();
            if (inventoryPlaceableItem == null) return;
            service?.beginDrag(inventoryPlaceableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            service?.updateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            service?.endDrag(eventData);
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