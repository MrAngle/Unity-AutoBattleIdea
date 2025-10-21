using System;
using Inventory.Slots.Context;
using Inventory.Slots.View;
using UnityEngine;
using Zenject;

namespace Inventory.Slots {
    public class InventoryPanelPrefabInitializer : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private InventoryGridView gridViewPrefab;
        [SerializeField] private InventoryCellView cellViewPrefab;

        [Header("Grid Size")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 6;

        private InventoryGridView _gridView;
        // private InventoryGrid _model;
        [Inject] private InventoryGridContext _inventoryGridContext;
        
        // public event Action<InventoryGrid> OnReady;
        // public bool IsReady { get; private set; }

        private void Awake() {
            var inventoryGrid = new InventoryGrid(width, height);
            _inventoryGridContext.SetInventoryGrid(inventoryGrid);
        }
        
        private void Start()
        {
            // 1) Model
            // _model = new InventoryGrid(width, height);

            // 2) Widok – instancja jako dziecko InventorySection
            _gridView = Instantiate(gridViewPrefab, transform, false);

            // wstrzyknij referencje do prefabu komórki (jeśli nie ustawione w prefabie)
            var field = _gridView.GetType()
                .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, cellViewPrefab);

            // 3) Zbuduj siatkę
            _gridView.Build(_inventoryGridContext.GetInventoryGrid());

            // 4) na wszelki wypadek rozciągnij widok do panelu
            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            //
            // IsReady = true;
            // OnReady?.Invoke(_model);
        }
    }
}