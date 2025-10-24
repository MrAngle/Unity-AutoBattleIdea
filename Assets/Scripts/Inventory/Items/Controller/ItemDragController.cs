using Config.Semantics;
using Inventory.Items.View;
using Inventory.Slots;
using Inventory.Slots.Context;
using UI.Inventory.Items.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Inventory.Items.Controller {
    public class ItemDragController : MonoBehaviour {
        [Inject] private readonly ItemsLayerRectTransform _itemsLayer;

        [Inject] private InventoryGridLayoutGroup _inventoryGridLayout;
        [Inject] private InventoryPanelPrefabInitializer _inventoryPanelInitializer;

        [Inject] private readonly InventoryGridContext _inventoryGridContext;
        
        [Inject] private readonly DragGhostPrefabItemView _dragGhostPrefabItemView;
        [Inject] private readonly ItemViewPrefabItemView _itemViewPrefabItemView;
        
        private ItemData _dragData;
        private ItemView _ghostItem;

        private void Start() {
            // model zbudowany w Start() panelInit
            // var field = typeof(InventoryPanelPrefabInitializer)
            //     .GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
            // _model = (InventoryGrid)field.GetValue(panelInit);

            _ghostItem = Instantiate(_dragGhostPrefabItemView.Get(), _itemsLayer.Get(), false);
            _ghostItem.gameObject.SetActive(false);
        }

        public void BeginDrag(ItemData data, PointerEventData e) {
            _dragData = data;
            var cellSize = _inventoryGridLayout.Get().cellSize;
            _ghostItem.Build(_dragData, cellSize);
            _ghostItem.SetColor(new Color(1f, 1f, 1f, 0.6f));
            _ghostItem.gameObject.SetActive(true);

            UpdateDrag(e);
        }

        public void UpdateDrag(PointerEventData e) {
            if (_dragData == null) return;

            // 1) pozycja kursora w układzie ItemsLayer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), e.position, e.pressEventCamera, out var localPos);

            // 2) zamiana na origin komórkowy
            var cell = _inventoryGridLayout.Get().cellSize;
            var spacing = _inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y)); // pivot (0,1) -> oś Y w dół

            var origin = new Vector2Int(x, y);

            var inventoryGrid = _inventoryGridContext.GetInventoryGrid();

            // 3) validacja
            var can = inventoryGrid != null && inventoryGrid.CanPlace(_dragData, origin);
            _ghostItem.SetColor(can ? new Color(0.5f, 1f, 0.5f, 0.7f) : new Color(1f, 0.5f, 0.5f, 0.7f));

            // 4) ustaw „ducha” na snapniętej pozycji
            _ghostItem.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);
        }

        public void EndDrag(PointerEventData e) {
            if (_dragData == null) {
                _ghostItem.gameObject.SetActive(false);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), e.position, e.pressEventCamera, out var localPos);

            var cell = _inventoryGridLayout.Get().cellSize;
            var spacing = _inventoryGridLayout.Get().spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y));
            var origin = new Vector2Int(x, y);
            
            var inventoryGrid = _inventoryGridContext.GetInventoryGrid();

            if (inventoryGrid != null && inventoryGrid.CanPlace(_dragData, origin)) {
                inventoryGrid.Place(_dragData, origin);
                var view = Instantiate(_itemViewPrefabItemView.Get(), _itemsLayer.Get(), false);
                view.Build(_dragData, cell);
                view.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);
            }

            _ghostItem.gameObject.SetActive(false);
            _dragData = null;
        }
    }
}