using Combat.Flow.Domain.Aggregate;
using Config.Semantics;
using Inventory;
using Inventory.EntryPoints;
using Inventory.Items;
using Inventory.Items.View;
using Inventory.Slots;
using Inventory.Slots.Context;
using Inventory.Slots.Domain;
using Inventory.Slots.View;
using UnityEngine;
using UnityEngine.Serialization;
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

        // [Header("Scripts")]
        // [SerializeField] private InventoryPanelPrefabInitializer inventoryPanelPrefabInitializer;
        

        public override void InstallBindings()
        {
            InstallSignals();
            
            BindItemsLayerRectTransform();
            
            BindInventoryGridLayoutGroup();
            
            Container.BindInterfacesAndSelfTo<InventoryViewPresenter>()
                .AsSingle();

            Container.Bind<InventoryGridContext>()
                .FromMethod(_ => InventoryGridContext.Create())
                .AsSingle()
                .NonLazy();
            
            BindFactories();
            
            Container.Bind<InventoryAggregateContext>()
                .AsSingle()
                .NonLazy();
            
            // PREFAB INITIALIZER
            Container.Bind<ItemViewPrefabItemView>()
                .FromMethod(_ => new ItemViewPrefabItemView(itemViewPrefab))
                .AsSingle();
            
            Container.Bind<DragGhostPrefabItemView>()
                .FromMethod(_ => new DragGhostPrefabItemView(dragGhostPrefab))
                .AsSingle();
            

            
            Container.Bind<GridViewPrefabInventoryGridView>()
                .FromMethod(_ => new GridViewPrefabInventoryGridView(gridViewPrefab))
                .AsSingle();

            Container.Bind<CellViewPrefabInventoryCellView>()
                .FromMethod(_ => new CellViewPrefabInventoryCellView(cellViewPrefab))
                .AsSingle();

 
            // GRID LAYOUT
  
            
            // SCRIPTS
            // Container.Bind<InventoryPanelPrefabInitializer>()
            //     .FromComponentInHierarchy()
            //     .AsSingle();


        }

        private void BindFactories() {
            Container.Bind<IFlowFactory>()
                .To<FlowFactory>()
                .AsSingle();

            Container.Bind<IEntryPointFactory>()
                .To<EntryPointFactory>()
                .AsSingle();

            Container.Bind<IInventoryAggregateFactory>()
                .To<InventoryAggregateFactory>()
                .AsSingle();
            
            
            Container.Bind<IItemViewFactory>()
                .To<ItemViewFactory>()
                .AsSingle();
        }

        private void BindInventoryGridLayoutGroup() {
            Container.Bind<InventoryGridLayoutGroup>()
                .FromMethod(_ => new InventoryGridLayoutGroup(inventoryGridLayout))
                .AsSingle()
                .NonLazy();
        }

        private void BindItemsLayerRectTransform() {
            Container.Bind<ItemsLayerRectTransform>()
                .FromMethod(_ => new ItemsLayerRectTransform(itemsLayerRectTransform))
                .AsSingle()
                .NonLazy();
        }

        void InstallSignals() {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<ItemPlacedDtoEvent>();
            Container.DeclareSignal<ItemRemovedDtoEvent>();
            Container.DeclareSignal<ItemPowerChangedDtoEvent>();


        }
    }
}