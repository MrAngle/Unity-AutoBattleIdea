using MageFactory.Item.Catalog;
using MageFactory.Shared.Contract;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private ItemDragController _controller;

        // private IItemFactory itemFactory;
        private IItemDefinition inventoryPlaceableItem;

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
        // private IInventoryPlaceableItem inventoryPlaceableItem;

        [Inject]
        public void construct(
            // IItemFactory injectItemFactory
        ) {
            // itemFactory = NullGuard.NotNullOrThrow(injectItemFactory);
        }


        private IItemDefinition getRandomItemDefinition() {
            return EntryPointDefinition.All[Random.Range(0, EntryPointDefinition.All.Count)];

            // return itemFactory.createPlacableItem(new CreatePlaceableItemCommand(shapeArchetype));
            // return new EquipItemCommand(shapeArchetype, );
        }
    }
}