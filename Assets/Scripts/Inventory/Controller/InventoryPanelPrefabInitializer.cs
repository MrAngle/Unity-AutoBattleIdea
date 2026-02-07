using System;
using System.Reflection;
using MageFactory.Context;
using MageFactory.Item.Controller.Api;
using MageFactory.Item.Controller.Domain;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class InventoryPanelPrefabInitializer : MonoBehaviour {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private CellViewPrefabInventoryCellView _cellViewPrefab;

        private InventoryGridView _gridView;

        [Header("Prefabs")] [Inject] private GridViewPrefabInventoryGridView _gridViewPrefab;

        [Inject] private InventoryAggregateContext _inventoryAggregateContext;

        [Inject] private InventoryGridContext _inventoryGridContext;

        private void Awake() {
            _gridView = Instantiate(_gridViewPrefab.Get(), transform, false);

            var field = _gridView.GetType()
                .GetField("cellPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, _cellViewPrefab.Get());
        }

        private void OnEnable() {
            _inventoryGridContext.InventoryGridChanged += OnInventoryGridChanged;

            var current = _inventoryGridContext.getInventoryGrid();
            if (current != null)
                OnInventoryGridChanged(current);
        }

        private void OnDisable() {
            _inventoryGridContext.InventoryGridChanged -= OnInventoryGridChanged;
        }

        public void Initialize() {
            _signalBus.Subscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            _signalBus.Subscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.Subscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        private void OnItemPlaced() {
            throw new NotImplementedException();
        }

        private void OnItemRemoved(ItemRemovedDtoEvent itemRemovedEvent) {
            throw new NotImplementedException();
        }

        private void OnPowerChanged(ItemPowerChangedDtoEvent changedDtoEvent) {
            throw new NotImplementedException();
        }


        public void Dispose() {
            _signalBus.TryUnsubscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            _signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        private void OnInventoryGridChanged(IInventoryGrid grid) {
            _gridView.Build((InventoryGrid)grid);

            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}