using MageFactory.Context;
using MageFactory.Item.Controller.Api;
using MageFactory.Semantics;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class ItemDragController : MonoBehaviour {
        private ItemsLayerRectTransform itemsLayer;
        private InventoryGridLayoutGroup inventoryGridLayout;
        private CharacterAggregateContext characterAggregateContext;
        private DragGhostPrefabItemView dragGhostPrefabItemView;

        private PlacedItemView ghostPlacedItem;
        private IPlaceableItem _placeableItem;

        [Inject]
        public void construct(
            ItemsLayerRectTransform injectItemsLayer,
            InventoryGridLayoutGroup injectInventoryGridLayout,
            CharacterAggregateContext injectCharacterAggregateContext,
            DragGhostPrefabItemView injectDragGhostPrefabItemView
        ) {
            itemsLayer = NullGuard.NotNullOrThrow(injectItemsLayer);
            inventoryGridLayout = NullGuard.NotNullOrThrow(injectInventoryGridLayout);
            characterAggregateContext = NullGuard.NotNullOrThrow(injectCharacterAggregateContext);
            dragGhostPrefabItemView = NullGuard.NotNullOrThrow(injectDragGhostPrefabItemView);
        }

        private void Start() {
            ghostPlacedItem = Instantiate(dragGhostPrefabItemView.Get(), itemsLayer.Get(), false);
            ghostPlacedItem.gameObject.SetActive(false);
        }

        internal void beginDrag(IPlaceableItem data, PointerEventData eventData) {
            _placeableItem = data;
            var cellSize = inventoryGridLayout.Get().cellSize;
            ghostPlacedItem.build(_placeableItem.getShape(), cellSize);
            ghostPlacedItem.setColor(new Color(1f, 1f, 1f, 0.6f));
            ghostPlacedItem.gameObject.SetActive(true);

            updateDrag(eventData);
        }

        internal void updateDrag(PointerEventData pointerEventData) {
            if (_placeableItem == null) return;

            // 1) pozycja kursora w układzie ItemsLayer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                itemsLayer.Get(), pointerEventData.position, pointerEventData.pressEventCamera, out var localPos);

            // 2) zamiana na origin komórkowy
            var cell = inventoryGridLayout.Get().cellSize;
            var spacing = inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y)); // pivot (0,1) -> oś Y w dół

            var origin = new Vector2Int(x, y);
            var characterAggregateContext = this.characterAggregateContext.getCharacterAggregateContext();

            // 3) validacja
            var can = characterAggregateContext != null &&
                      characterAggregateContext.canPlaceItem(_placeableItem, origin);
            ghostPlacedItem.setColor(can ? new Color(0.5f, 1f, 0.5f, 0.7f) : new Color(1f, 0.5f, 0.5f, 0.7f));

            // 4) ustaw „ducha” na snapniętej pozycji
            ghostPlacedItem.setOriginInGrid(origin, cell, Vector2.zero, spacing.x);
        }

        internal void endDrag(PointerEventData pointerEventData) {
            if (_placeableItem == null) {
                ghostPlacedItem.gameObject.SetActive(false);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                itemsLayer.Get(), pointerEventData.position, pointerEventData.pressEventCamera, out var localPos);

            var cell = inventoryGridLayout.Get().cellSize;
            var spacing = inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y));
            var origin = new Vector2Int(x, y);

            var characterAggregateContext = this.characterAggregateContext.getCharacterAggregateContext();

            if (characterAggregateContext != null
                && characterAggregateContext.equipItemOrThrow(_placeableItem, origin, out var placedItem))
                NullGuard.NotNullCheckOrThrow(placedItem);

            // ICharacterInventoryFacade inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();
            // if (characterAggregateContext != null && characterAggregateContext.CanPlace(_placeableItem, origin)) {
            //     IPlacedItem placedItem = inventoryAggregate.Place(_placeableItem, origin);
            // }

            ghostPlacedItem.gameObject.SetActive(false);
            _placeableItem = null;
        }
    }
}