using System;
using System.Linq;
using Inventory.Items.Config;
using Inventory.Items.Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Items.Controller {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("Config")] public ShapeArchetypeId itemId = ShapeArchetypeId.SQUARE_2X2;

        private ItemDragController _controller;
        // private ShapeArchetype _data;
        private IPlaceableItem _placeableItem;

        private void Awake() {
            _controller = FindObjectOfType<ItemDragController>(true);
            // Prosty lookup z Twojej statycznej konfiguracji:
            ShapeArchetype shapeArchetype = Array.Find(ShapeCatalog.All.ToArray(), d => d.ShapeArchetypeId == itemId);
            ItemArchetype itemArchetype = new ItemArchetype(shapeArchetype);

            _placeableItem = itemArchetype;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (_placeableItem == null) return;
            _controller?.BeginDrag(_placeableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_placeableItem == null) return;
            _controller?.UpdateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (_placeableItem == null) return;
            _controller?.EndDrag(eventData);
        }
    }
}