using System;
using Combat.Flow.Domain.Aggregate;
using Config.Semantics;
using Context;
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
        [Inject] private InventoryAggregateContext _inventoryAggregateContext;
        
        // [Header("Grid Size")]
        // [SerializeField] private int width = 8;
        // [SerializeField] private int height = 6;

        private InventoryGridView _gridView;



        // public void Awake() {
        //     // ICharacterInventoryFacade inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregate();
        //     //
        //     // // IEntryPointFacade entryPoint = GridEntryPoint.Create(FlowKind.Damage, new Vector2Int(0, 0));
        //     // //
        //     // // IInventoryGrid inventoryGrid = IInventoryGrid.CreateInventoryGrid(width, height, entryPoint);
        //     // _inventoryGridContext.SetInventoryGrid(inventoryAggregate.GetInventoryGrid());
        // }
        
        
        private void Awake() {
            // jeśli chcesz – stworzenie widoku raz
            _gridView = Instantiate(_gridViewPrefab.Get(), transform, false);

            var field = _gridView.GetType()
                .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(_gridView) == null)
                field.SetValue(_gridView, _cellViewPrefab.Get());

            // początkowe podpięcie inventory (np. aktualnie wybranego charactera)
            // var inventory = _inventoryAggregateContext.GetInventoryAggregateContext()
            //     .GetInventoryGrid();
            //
            // _inventoryGridContext.SetInventoryGrid(inventory);
        }
        
        private void Start() {
            // ICharacterInventoryFacade inventoryAggregate = _inventoryAggregateContext.GetInventoryAggregateContext();
            // _inventoryGridContext.SetInventoryGrid(inventoryAggregate.GetInventoryGrid());
            //
            // _gridView = Instantiate(_gridViewPrefab.Get(), transform, false);
            //
            // var field = _gridView.GetType()
            //     .GetField("cellPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // if (field != null && field.GetValue(_gridView) == null)
            //     field.SetValue(_gridView, _cellViewPrefab.Get());
            //
            // _gridView.Build(_inventoryGridContext.GetInventoryGrid());
            //
            // var rt = (RectTransform)_gridView.transform;
            // rt.anchorMin = Vector2.zero;
            // rt.anchorMax = Vector2.one;
            // rt.offsetMin = Vector2.zero;
            // rt.offsetMax = Vector2.zero;
        }
        
        private void OnEnable()
        {
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