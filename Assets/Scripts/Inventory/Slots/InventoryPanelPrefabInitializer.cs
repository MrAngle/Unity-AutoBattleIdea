using System;
using Combat.Flow.Domain.Aggregate;
using Config.Semantics;
using Inventory.EntryPoints;
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
        
        [Header("Grid Size")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 6;

        private InventoryGridView _gridView;



        private void Awake() {
            IEntryPointFacade entryPoint = GridEntryPoint.Create(FlowKind.Damage, new Vector2Int(0, 0));

            var inventoryGrid = new InventoryGrid(width, height, entryPoint);
            _inventoryGridContext.SetInventoryGrid(inventoryGrid);
        }
        
        private void Start()
        {
            // 1) Model
            // _model = new InventoryGrid(width, height);

            // 2) Widok – instancja jako dziecko InventorySection
            _gridView = Instantiate(_gridViewPrefab.Get(), transform, false);

            // wstrzyknij referencje do prefabu komórki (jeśli nie ustawione w prefabie)
            var field = _gridView.GetType()
                .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, _cellViewPrefab.Get());

            // 3) Zbuduj siatkę
            _gridView.Build(_inventoryGridContext.GetInventoryGrid());

            // 4) na wszelki wypadek rozciągnij widok do panelu
            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}