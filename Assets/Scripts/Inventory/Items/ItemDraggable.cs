using System;
using System.Linq;
using UI.Inventory.Items.Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Items {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("Config")] public string itemId; // np. "l_bracket" z ItemConfig

        private ItemDragController _controller;
        private ItemData _data;

        private void Awake() {
            _controller = FindObjectOfType<ItemDragController>(true);
            // Prosty lookup z Twojej statycznej konfiguracji:
            _data = Array.Find(ItemConfig.All.ToArray(), d => d.Id == itemId);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (_data == null) return;
            _controller?.BeginDrag(_data, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_data == null) return;
            _controller?.UpdateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (_data == null) return;
            _controller?.EndDrag(eventData);
        }
    }
}