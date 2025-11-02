using Config.Semantics;
using Inventory.Items.View;
using Inventory.Slots;
using Inventory.Slots.Context;
using Inventory.Items.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Inventory.Items.Controller {
    public class ItemDragController : MonoBehaviour {
        [Inject] private readonly ItemsLayerRectTransform _itemsLayer;

        [Inject] private readonly InventoryGridLayoutGroup _inventoryGridLayout;
        [Inject] private readonly InventoryAggregateContext _inventoryAggregateContext;
        
        [Inject] private readonly DragGhostPrefabItemView _dragGhostPrefabItemView;
        [Inject] private readonly ItemViewPrefabItemView _itemViewPrefabItemView;
        
        // private ShapeArchetype _shapeArchetype;
        private IPlaceableItem _placeableItem;
        private ItemView _ghostItem;

        private void Start() {
            _ghostItem = Instantiate(_dragGhostPrefabItemView.Get(), _itemsLayer.Get(), false);
            _ghostItem.gameObject.SetActive(false);
        }

        public void BeginDrag(IPlaceableItem data, PointerEventData eventData) {
            _placeableItem = data;
            var cellSize = _inventoryGridLayout.Get().cellSize;
            _ghostItem.Build(_placeableItem.GetShape(), cellSize);
            _ghostItem.SetColor(new Color(1f, 1f, 1f, 0.6f));
            _ghostItem.gameObject.SetActive(true);

            UpdateDrag(eventData);
        }

        public void UpdateDrag(PointerEventData pointerEventData) {
            if (_placeableItem == null) return;

            // 1) pozycja kursora w układzie ItemsLayer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), pointerEventData.position, pointerEventData.pressEventCamera, out var localPos);

            // 2) zamiana na origin komórkowy
            var cell = _inventoryGridLayout.Get().cellSize;
            var spacing = _inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y)); // pivot (0,1) -> oś Y w dół

            var origin = new Vector2Int(x, y);
            InventoryAggregate inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();

            // 3) validacja
            var can = inventoryAggregate != null && inventoryAggregate.CanPlace(_placeableItem, origin);
            _ghostItem.SetColor(can ? new Color(0.5f, 1f, 0.5f, 0.7f) : new Color(1f, 0.5f, 0.5f, 0.7f));

            // 4) ustaw „ducha” na snapniętej pozycji
            _ghostItem.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);
        }

        public void EndDrag(PointerEventData pointerEventData) {
            if (_placeableItem == null) {
                _ghostItem.gameObject.SetActive(false);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), pointerEventData.position, pointerEventData.pressEventCamera, out var localPos);

            var cell = _inventoryGridLayout.Get().cellSize;
            var spacing = _inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y));
            var origin = new Vector2Int(x, y);
            
            InventoryAggregate inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();

            if (inventoryAggregate != null && inventoryAggregate.CanPlace(_placeableItem, origin)) {
                IPlacedItem placedItem = inventoryAggregate.Place(_placeableItem, origin);
            }

            _ghostItem.gameObject.SetActive(false);
            _placeableItem = null;
        }
    }
}