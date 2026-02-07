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
        [Header("Config")] public ShapeArchetypeId itemId = ShapeArchetypeId.SQUARE_2X2;
        private IPlaceableItem placeableItem;

        private void Awake() {
            _controller = FindAnyObjectByType<ItemDragController>(FindObjectsInactive.Include);

            // var shapeArchetype = Array.Find(ShapeCatalog.All.ToArray(), d => d.ShapeArchetypeId == itemId);
            // var shapeArchetype = ShapeCatalog.All[UnityEngine.Random.Range(0, ShapeCatalog.All.Count)];
            //
            // var itemArchetype = new ItemArchetype(shapeArchetype);
            //
            // _placeableItem = itemArchetype;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            placeableItem = getRandomItem();
            if (placeableItem == null) return;
            _controller?.BeginDrag(placeableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (placeableItem == null) return;
            _controller?.UpdateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (placeableItem == null) return;
            _controller?.EndDrag(eventData);
            placeableItem = null;
        }

        [Inject]
        public void Construct(
            IItemFactory itemFactory
        ) {
            this.itemFactory = NullGuard.NotNullOrThrow(itemFactory);
        }

        private IPlaceableItem getRandomItem() {
            var shapeArchetype = ShapeCatalog.All[Random.Range(0, ShapeCatalog.All.Count)];

            return itemFactory.createPlacableItem(new CreatePlaceableItemCommand(shapeArchetype));
        }
    }
}