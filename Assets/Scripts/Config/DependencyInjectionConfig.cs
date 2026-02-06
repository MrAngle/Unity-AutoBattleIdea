using Inventory.Controller.Items.View;
using Inventory.Items.View;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Domain;
using MageFactory.BattleManager;
using MageFactory.Character.Api;
using MageFactory.Character.Controller;
using MageFactory.Character.Domain.Service;
using MageFactory.Context;
using MageFactory.Flow.Api;
using MageFactory.Flow.Domain.Service;
using MageFactory.Inventory.Api;
using MageFactory.Inventory.Domain.Service;
using Semantics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Config {
    public class DependencyInjectionConfig : MonoInstaller {
        // [SerializeField] private int width = 8;

        [Header("Prefabs")] [SerializeField] private ItemView itemViewPrefab;
        [SerializeField] private ItemView dragGhostPrefab;
        [SerializeField] private InventoryGridView gridViewPrefab;
        [SerializeField] private InventoryCellView cellViewPrefab;

        [Header("RectTransforms")] [SerializeField]
        private RectTransform itemsLayerRectTransform;

        [Header("GridLayoutGroup")] [SerializeField]
        private GridLayoutGroup inventoryGridLayout;

        [Header("Battle UI")] [SerializeField] private CharacterPrefabAggregate battleSlotPrefab;

        [SerializeField] private Transform battleSlotParent;
        // private InventoryCellView cellViewPrefab;
        // private ItemView dragGhostPrefab;
        //
        // private InventoryGridView gridViewPrefab;
        // // [SerializeField] private int width = 8;
        //
        // [Header("Prefabs")] private ItemView itemViewPrefab;

        // private ItemView dragGhostPrefab;
        // // [SerializeField] private int width = 8;
        //
        // [Header("Prefabs")] private ItemView itemViewPrefab;

        // [Header("Scripts")]
        // [SerializeField] private InventoryPanelPrefabInitializer inventoryPanelPrefabInitializer;


        public override void InstallBindings() {
            InstallSignals();

            BindItemsLayerRectTransform();

            BindInventoryGridLayoutGroup();


            BindContexts();

            BindFactories();

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

            Container.BindInterfacesAndSelfTo<InventoryViewPresenter>()
                .AsSingle()
                .NonLazy();


            // Container.Bind<InventoryAggregateContext>()
            //     .AsSingle()
            //     .NonLazy();


            // GRID LAYOUT


            // SCRIPTS
            // Container.Bind<InventoryPanelPrefabInitializer>()
            //     .FromComponentInHierarchy()
            //     .AsSingle();
        }

        private void BindContexts() {
            Container.Bind<InventoryGridContext>()
                .AsSingle()
                .NonLazy();

            Container.Bind<InventoryAggregateContext>()
                .AsSingle()
                .NonLazy();

            Container.Bind<CharacterAggregateContext>()
                .AsSingle()
                .NonLazy();
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

            Container.Bind<IActionExecutor>()
                .To<ActionExecutorService>()
                .AsSingle();

            Container.Bind<ICharacterAggregateFactory>()
                .To<CharacterAggregateFactory>()
                .AsSingle();

            BindCharactersAndBattleUI();
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

        private void BindCharactersAndBattleUI() {
            // prefab slotu
            Container.Bind<CharacterPrefabAggregate>()
                .FromInstance(battleSlotPrefab)
                .AsSingle();

            // parent do slotów – identyfikujemy go ID, bo Transformów jest mnóstwo
            Container.Bind<Transform>()
                .WithId("BattleSlotParent")
                .FromInstance(battleSlotParent)
                .AsSingle();

            // sam manager z hierarchii sceny
            Container.Bind<BattleUIManager>()
                .FromComponentInHierarchy()
                .AsSingle();
        }

        private void InstallSignals() {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<ItemPlacedDtoEvent>();
            Container.DeclareSignal<ItemRemovedDtoEvent>();
            Container.DeclareSignal<ItemPowerChangedDtoEvent>();
        }
    }
}