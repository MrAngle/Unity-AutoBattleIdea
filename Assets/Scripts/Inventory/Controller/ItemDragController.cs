using MageFactory.Context;
using MageFactory.Inventory.Api;
using MageFactory.Semantics;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Inventory.Controller {
    public class ItemDragController : MonoBehaviour {
        private CharacterAggregateContext _characterAggregateContext;
        private DragGhostPrefabItemView _dragGhostPrefabItemView;
        private ItemView _ghostItem;

        private InventoryGridLayoutGroup _inventoryGridLayout;

        private ItemsLayerRectTransform _itemsLayer;
        private ItemViewPrefabItemView _itemViewPrefabItemView;

        // private ShapeArchetype _shapeArchetype;
        private IPlaceableItem _placeableItem;


        private void Start() {
            _ghostItem = Instantiate(_dragGhostPrefabItemView.Get(), _itemsLayer.Get(), false);
            _ghostItem.gameObject.SetActive(false);
        }

        [Inject]
        public void Construct(
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup inventoryGridLayout,
            CharacterAggregateContext characterAggregateContext,
            DragGhostPrefabItemView dragGhostPrefabItemView,
            ItemViewPrefabItemView itemViewPrefabItemView
        ) {
            _itemsLayer = NullGuard.NotNullOrThrow(itemsLayer);
            _inventoryGridLayout = NullGuard.NotNullOrThrow(inventoryGridLayout);
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
            _dragGhostPrefabItemView = NullGuard.NotNullOrThrow(dragGhostPrefabItemView);
            _itemViewPrefabItemView = NullGuard.NotNullOrThrow(itemViewPrefabItemView);
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
            var characterAggregateContext = _characterAggregateContext.GetCharacterAggregateContext();

            // 3) validacja
            var can = characterAggregateContext != null &&
                      characterAggregateContext.canPlaceItem(_placeableItem, origin);
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

            var characterAggregateContext = _characterAggregateContext.GetCharacterAggregateContext();

            if (characterAggregateContext != null
                && characterAggregateContext.equipItemOrThrow(_placeableItem, origin, out var placedItem))
                NullGuard.NotNullCheckOrThrow(placedItem);

            // ICharacterInventoryFacade inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();
            // if (characterAggregateContext != null && characterAggregateContext.CanPlace(_placeableItem, origin)) {
            //     IPlacedItem placedItem = inventoryAggregate.Place(_placeableItem, origin);
            // }

            _ghostItem.gameObject.SetActive(false);
            _placeableItem = null;
        }
    }
}