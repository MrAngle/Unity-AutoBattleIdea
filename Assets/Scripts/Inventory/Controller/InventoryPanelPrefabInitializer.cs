using System;
using System.Reflection;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Context;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class InventoryPanelPrefabInitializer : MonoBehaviour {
        private SignalBus signalBus;
        private CellViewPrefabInventoryCellView cellViewPrefab;
        private GridViewPrefabInventoryGridView gridViewPrefab;
        private InventoryGridContext inventoryGridContext;

        private InventoryGridView _gridView;

        [Inject]
        private void construct(
            SignalBus injectSignalBus,
            CellViewPrefabInventoryCellView injectCellViewPrefab,
            GridViewPrefabInventoryGridView injectGridViewPrefab,
            InventoryGridContext injectInventoryGridContext
        ) {
            signalBus = NullGuard.NotNullOrThrow(injectSignalBus);
            cellViewPrefab = NullGuard.NotNullOrThrow(injectCellViewPrefab);
            gridViewPrefab = NullGuard.NotNullOrThrow(injectGridViewPrefab);
            inventoryGridContext = NullGuard.NotNullOrThrow(injectInventoryGridContext);
        }

        private void Awake() {
            _gridView = Instantiate(gridViewPrefab.Get(), transform, false);

            var field = _gridView.GetType()
                .GetField("cellPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, cellViewPrefab.Get());
        }

        private void OnEnable() {
            inventoryGridContext.InventoryGridChanged += OnInventoryGridChanged;

            var current = inventoryGridContext.getInventoryGrid();
            if (current != null)
                OnInventoryGridChanged(current);
        }

        private void OnDisable() {
            inventoryGridContext.InventoryGridChanged -= OnInventoryGridChanged;
        }

        private void initialize() {
            signalBus.Subscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            signalBus.Subscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            signalBus.Subscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
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

        public void dispose() {
            signalBus.TryUnsubscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        private void OnInventoryGridChanged(ICombatInventory grid) {
            _gridView.build(grid);

            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}