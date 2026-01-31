using System;
using Combat.Flow.Domain.Aggregate;
using Config.Semantics;
using Context;
using Inventory.EntryPoints;
using Inventory.Items.View;
using Inventory.Slots.Context;
using Inventory.Slots.Domain;
using Inventory.Slots.View;
using UnityEngine;
using Zenject;

namespace Inventory.Slots {
    public class InventoryPanelPrefabInitializer : MonoBehaviour
    {
        [Header("Prefabs")]
        [Inject] private GridViewPrefabInventoryGridView _gridViewPrefab;
        [Inject] private CellViewPrefabInventoryCellView _cellViewPrefab;

        [Inject] private InventoryGridContext _inventoryGridContext;
        [Inject] private InventoryAggregateContext _inventoryAggregateContext;
        [Inject] private readonly SignalBus _signalBus;

        private InventoryGridView _gridView;
        
        public void Initialize()
        {
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
        
        private void Awake() {
            _gridView = Instantiate(_gridViewPrefab.Get(), transform, false);

            var field = _gridView.GetType()
                .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, _cellViewPrefab.Get());
        }
        
        private void OnEnable() {
            _inventoryGridContext.InventoryGridChanged += OnInventoryGridChanged;

            var current = _inventoryGridContext.GetInventoryGrid();
            if (current != null)
                OnInventoryGridChanged(current);
        }

        private void OnDisable() {
            _inventoryGridContext.InventoryGridChanged -= OnInventoryGridChanged;
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