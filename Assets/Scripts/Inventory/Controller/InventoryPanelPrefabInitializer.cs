using System.Reflection;
using MageFactory.CombatContext.Contract;
using MageFactory.Context;
using MageFactory.Inventory.Api.Event;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public class InventoryPanelPrefabInitializer : MonoBehaviour, IInventoryChangedEventListener {
        private CellViewPrefabInventoryCellView cellViewPrefab;
        private GridViewPrefabInventoryGridView gridViewPrefab;
        private InventoryGridContext inventoryGridContext;
        private IInventoryEventHub inventoryEventHub;

        private InventoryGridView _gridView;

        [Inject]
        private void construct(
            CellViewPrefabInventoryCellView injectCellViewPrefab,
            GridViewPrefabInventoryGridView injectGridViewPrefab,
            InventoryGridContext injectInventoryGridContext,
            IInventoryEventHub injectInventoryEventHub
        ) {
            cellViewPrefab = NullGuard.NotNullOrThrow(injectCellViewPrefab);
            gridViewPrefab = NullGuard.NotNullOrThrow(injectGridViewPrefab);
            inventoryGridContext = NullGuard.NotNullOrThrow(injectInventoryGridContext);
            inventoryEventHub = NullGuard.NotNullOrThrow(injectInventoryEventHub);

            inventoryEventHub.subscribe(this);
        }

        private void Awake() {
            _gridView = Instantiate(gridViewPrefab.Get(), transform, false);

            FieldInfo fieldInfo = _gridView.GetType()
                .GetField("cellPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null && fieldInfo.GetValue(_gridView) == null)
                fieldInfo.SetValue(_gridView, cellViewPrefab.Get());
        }

        private void OnEnable() {
            inventoryGridContext.InventoryGridChanged += OnInventoryGridChanged;
        }

        // TODO: remove
        private void OnInventoryGridChanged(ICombatInventory grid) {
            _gridView.build(grid);

            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }


        // TODO: use it
        public void OnEvent(in InventoryChanged ev) {
            _gridView.build(ev.combatInventory);

            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}