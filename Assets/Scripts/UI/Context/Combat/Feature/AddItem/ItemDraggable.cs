using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Item.Catalog;
using MageFactory.Item.Catalog.Bases;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.UI.Context.Combat.Feature.AddItem {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private ItemDragService service;
        private IItemDefinition inventoryPlaceableItem;

        [Inject]
        public void construct(ItemDragService injectedService) {
            this.service = injectedService;
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
                new List<IItemDefinition> {
                    new RustySword(),
                    new Shield(),
                    EntryPointDefinition.Standard
                };

            // ItemDefinition.All
            //     .Concat<IItemDefinition>(EntryPointDefinition.All)
            //     .ToList();
            return allItems[Random.Range(0, allItems.Count)];
        }
    }
}