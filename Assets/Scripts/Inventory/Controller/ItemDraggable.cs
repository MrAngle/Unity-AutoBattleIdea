using MageFactory.Item.Api;
using MageFactory.Item.Api.Dto;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private ItemDragController _controller;
        private IItemFactory itemFactory;
        private IPlaceableItem placeableItem;

        [Inject]
        public void construct(
            IItemFactory injectItemFactory
        ) {
            itemFactory = NullGuard.NotNullOrThrow(injectItemFactory);
        }

        private void Awake() {
            _controller = FindAnyObjectByType<ItemDragController>(FindObjectsInactive.Include);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            placeableItem = getRandomItem();
            if (placeableItem == null) return;
            _controller?.beginDrag(placeableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (placeableItem == null) return;
            _controller?.updateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (placeableItem == null) return;
            _controller?.endDrag(eventData);
            placeableItem = null;
        }


        private IPlaceableItem getRandomItem() {
            var shapeArchetype = ShapeCatalog.All[Random.Range(0, ShapeCatalog.All.Count)];

            return itemFactory.createPlacableItem(new CreatePlaceableItemCommand(shapeArchetype));
        }
    }
}