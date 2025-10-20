using Inventory.Slots.View;
using UnityEngine;

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
        private InventoryGrid _model;

        private void Start()
        {
            // 1) Model
            _model = new InventoryGrid(width, height);

            // 2) Widok – instancja jako dziecko InventorySection
            _gridView = Instantiate(gridViewPrefab, transform, false);

            // wstrzyknij referencje do prefabu komórki (jeśli nie ustawione w prefabie)
            var field = _gridView.GetType()
                .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, cellViewPrefab);

            // 3) Zbuduj siatkę
            _gridView.Build(_model);

            // 4) na wszelki wypadek rozciągnij widok do panelu
            var rt = (RectTransform)_gridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}