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
        // [Header("Refs")] [SerializeField] private RectTransform itemsLayer;
        [Inject] private ItemsLayerRectTransform _itemsLayer;

        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private InventoryPanelPrefabInitializer panelInit;
        private ItemData _dragData;
        private ItemView _ghost;

        // private InventoryGrid _model;
        [Inject] private InventoryGridContext _inventoryGridContext;
        
        [SerializeField] private ItemView dragGhostPrefab;
        [SerializeField] private ItemView itemViewPrefab;
        
        

        private void Start() {
            // model zbudowany w Start() panelInit
            // var field = typeof(InventoryPanelPrefabInitializer)
            //     .GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
            // _model = (InventoryGrid)field.GetValue(panelInit);

            _ghost = Instantiate(dragGhostPrefab, _itemsLayer.Get(), false);
            _ghost.gameObject.SetActive(false);
        }
        
        // private void OnEnable() {
        //     if (panelInit != null) {
        //         panelInit.OnReady += HandlePanelReady;
        //     }
        // }
        //
        // private void OnDisable() {
        //     if (panelInit != null) {
        //         panelInit.OnReady -= HandlePanelReady;
        //     }
        // }
        
        // private void HandlePanelReady(InventoryGrid model) {
        //     _model = model;
        //     // jeśli ghost/preview wymaga modelu do inicjalizacji – zrób to tutaj
        // }

        public void BeginDrag(ItemData data, PointerEventData e) {
            _dragData = data;
            var cellSize = gridLayout.cellSize;
            _ghost.Build(_dragData, cellSize);
            _ghost.SetColor(new Color(1f, 1f, 1f, 0.6f));
            _ghost.gameObject.SetActive(true);

            UpdateDrag(e);
        }

        public void UpdateDrag(PointerEventData e) {
            if (_dragData == null) return;

            // 1) pozycja kursora w układzie ItemsLayer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), e.position, e.pressEventCamera, out var localPos);

            // 2) zamiana na origin komórkowy
            var cell = gridLayout.cellSize;
            var spacing = gridLayout.spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y)); // pivot (0,1) -> oś Y w dół

            var origin = new Vector2Int(x, y);

            var inventoryGrid = _inventoryGridContext.GetInventoryGrid();

            // 3) validacja
            var can = inventoryGrid != null && inventoryGrid.CanPlace(_dragData, origin);
            _ghost.SetColor(can ? new Color(0.5f, 1f, 0.5f, 0.7f) : new Color(1f, 0.5f, 0.5f, 0.7f));

            // 4) ustaw „ducha” na snapniętej pozycji
            _ghost.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);
        }

        public void EndDrag(PointerEventData e) {
            if (_dragData == null) {
                _ghost.gameObject.SetActive(false);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _itemsLayer.Get(), e.position, e.pressEventCamera, out var localPos);

            var cell = gridLayout.cellSize;
            var spacing = gridLayout.spacing;
            var x = Mathf.FloorToInt(localPos.x / (cell.x + spacing.x));
            var y = Mathf.FloorToInt(-localPos.y / (cell.y + spacing.y));
            var origin = new Vector2Int(x, y);
            
            var inventoryGrid = _inventoryGridContext.GetInventoryGrid();

            if (inventoryGrid != null && inventoryGrid.CanPlace(_dragData, origin)) {
                inventoryGrid.Place(_dragData, origin);
                var view = Instantiate(itemViewPrefab, _itemsLayer.Get(), false);
                view.Build(_dragData, cell);
                view.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);
            }

            _ghost.gameObject.SetActive(false);
            _dragData = null;
        }
    }
}