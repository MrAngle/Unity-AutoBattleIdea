using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Semantics;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component;
using MageFactory.UI.Component.Inventory.ItemLayer;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.UI.Context.Combat.Feature.AddItem {
    public class ItemDragService {
        private readonly PlacedItemView ghostPlacedItem;
        private readonly InventoryGridLayoutGroup inventoryGridLayout;
        private readonly ItemsLayerRectTransform itemsLayer;

        private IItemDefinition inventoryPlaceableItem; // todo change name
        private ICombatCharacter characterContext;

        [Inject]
        public ItemDragService(
            ItemsLayerRectTransform injectItemsLayer,
            InventoryGridLayoutGroup injectInventoryGridLayout,
            DragGhostPrefabItemView injectDragGhostPrefabItemView
        ) {
            NullGuard.NotNullOrThrow(injectDragGhostPrefabItemView);
            itemsLayer = NullGuard.NotNullOrThrow(injectItemsLayer);
            inventoryGridLayout = NullGuard.NotNullOrThrow(injectInventoryGridLayout);

            ghostPlacedItem = Object.Instantiate(injectDragGhostPrefabItemView.Get(), itemsLayer.Get(), false);
            ghostPlacedItem.gameObject.SetActive(false);
        }

        public void setCharacterContext(ICombatCharacter characterContext) {
            this.characterContext = characterContext;
        }

        internal void beginDrag(IItemDefinition itemDefinition, PointerEventData eventData) {
            inventoryPlaceableItem = itemDefinition;
            var cellSize = inventoryGridLayout.Get().cellSize;
            ghostPlacedItem.build(inventoryPlaceableItem.getShape(), cellSize);
            ghostPlacedItem.setColor(new Color(1f, 1f, 1f, 0.6f));
            ghostPlacedItem.gameObject.SetActive(true);

            updateDrag(eventData);
        }

        internal void updateDrag(PointerEventData pointerEventData) {
            if (inventoryPlaceableItem == null) return;

            // 1) pozycja kursora w układzie ItemsLayer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                itemsLayer.Get(), pointerEventData.position, pointerEventData.pressEventCamera, out var localPos);

            // 2) zamiana na origin komórkowy
            var cell = inventoryGridLayout.Get().cellSize;
            var spacing = inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y)); // pivot (0,1) -> oś Y w dół

            var origin = new Vector2Int(x, y);
            var characterAggregateContext = characterContext;

            // 3) validacja
            var can = characterAggregateContext != null &&
                      // characterAggregateContext.getInventoryAggregate()
                      //     .canPlace(new PlaceItemQuery(inventoryPlaceableItem, origin));
                      characterAggregateContext.canPlaceItem(new EquipItemQuery(inventoryPlaceableItem, origin));
            ghostPlacedItem.setColor(can ? new Color(0.5f, 1f, 0.5f, 0.7f) : new Color(1f, 0.5f, 0.5f, 0.7f));

            // 4) ustaw „ducha” na snapniętej pozycji
            ghostPlacedItem.setOriginInGrid(origin, cell, Vector2.zero, spacing.x);
        }

        internal void endDrag(PointerEventData pointerEventData) {
            if (inventoryPlaceableItem == null) {
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

            ICombatCharacter character = characterContext;
            if (character != null) {
                var equippedItem = character.equipItemOrThrow(new EquipItemCommand(inventoryPlaceableItem, origin));
                NullGuard.NotNullCheckOrThrow(equippedItem);
            }


            // ICharacterInventoryFacade inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();
            // if (characterAggregateContext != null && characterAggregateContext.CanPlace(_placeableItem, origin)) {
            //     IPlacedItem placedItem = inventoryAggregate.Place(_placeableItem, origin);
            // }

            ghostPlacedItem.gameObject.SetActive(false);
            inventoryPlaceableItem = null;
        }
    }
}