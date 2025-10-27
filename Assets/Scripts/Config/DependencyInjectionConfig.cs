using Config.Semantics;
using Inventory;
using Inventory.Items.View;
using Inventory.Slots;
using Inventory.Slots.Context;
using Inventory.Slots.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Config {
    public class DependencyInjectionConfig: MonoInstaller {
        // [SerializeField] private int width = 8;

        [Header("Prefabs")]
        [SerializeField] private ItemView itemViewPrefab;
        [SerializeField] private ItemView dragGhostPrefab;
        [SerializeField] private InventoryGridView gridViewPrefab; 
        [SerializeField] private InventoryCellView cellViewPrefab;
        
        [Header("RectTransforms")]
        [SerializeField] private RectTransform itemsLayerRectTransform;

        [Header("GridLayoutGroup")]
        [SerializeField] private GridLayoutGroup inventoryGridLayout;
        
        [Header("Scripts")]
        [SerializeField] private InventoryPanelPrefabInitializer _inventoryPanelPrefabInitializer;
        

        public override void InstallBindings()
        {
            Container.Bind<InventoryGridContext>()
                .FromMethod(_ => InventoryGridContext.Create())
                .AsSingle()
                .NonLazy();
            
            Container.Bind<InventoryAggregateContext>()
                .FromMethod(_ => InventoryAggregateContext.Create())
                .AsSingle()
                .NonLazy();
            
            // PREFAB INITIALIZER
            Container.Bind<ItemViewPrefabItemView>()
                .FromMethod(_ => new ItemViewPrefabItemView(itemViewPrefab))
                .AsSingle();
            
            Container.Bind<DragGhostPrefabItemView>()
                .FromMethod(_ => new DragGhostPrefabItemView(dragGhostPrefab))
                .AsSingle();
            
            Container.Bind<ItemsLayerRectTransform>()
                .FromMethod(_ => new ItemsLayerRectTransform(itemsLayerRectTransform))
                .AsSingle()
                .NonLazy();
            
            Container.Bind<GridViewPrefabInventoryGridView>()
                .FromMethod(_ => new GridViewPrefabInventoryGridView(gridViewPrefab))
                .AsSingle();

            Container.Bind<CellViewPrefabInventoryCellView>()
                .FromMethod(_ => new CellViewPrefabInventoryCellView(cellViewPrefab))
                .AsSingle();

 
            // GRID LAYOUT
            Container.Bind<InventoryGridLayoutGroup>()
                .FromMethod(_ => new InventoryGridLayoutGroup(inventoryGridLayout))
                .AsSingle()
                .NonLazy();
            
            // SCRIPTS
            Container.Bind<InventoryPanelPrefabInitializer>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}