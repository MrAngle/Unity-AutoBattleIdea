using System;
using MageFactory.BattleManager;
using MageFactory.Semantics;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component;
using MageFactory.UI.Component.Inventory;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using MageFactory.UI.Context.Combat;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Context.Combat.Feature.AddItem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.InjectConfiguration {
    public sealed class
        MageFactoryUnityUiInstaller : Installer<MageFactoryUnityUiInstaller.Settings, MageFactoryUnityUiInstaller> {
        [Serializable]
        public sealed class Settings {
            [Header("Prefabs")] public PlacedItemView placedItemViewPrefab;
            public PlacedItemView dragGhostPrefab;
            public InventoryGridView gridViewPrefab;
            public InventoryCellView cellViewPrefab;

            [Header("RectTransforms")] public RectTransform itemsLayerRectTransform;

            [Header("GridLayoutGroup")] public GridLayoutGroup inventoryGridLayout;

            [Header("Battle UI")] public CharacterPrefabAggregate battleSlotPrefab;
            public Transform battleSlotParent;
        }

        private readonly Settings settings;

        public MageFactoryUnityUiInstaller(Settings settings) {
            this.settings = NullGuard.NotNullOrThrow(settings);
        }

        public override void InstallBindings() {
            bindUiSemanticsAndPrefabProviders();
            bindUiPanelsAndPresenters();
            bindUiFactories();
            bindUiEventHub();
            bindBattleUi();
            bindRuntimeServices();
        }

        private void bindRuntimeServices() {
            // Runtime tick logic, nadal "bez UI", ale zwykle wygodnie mieć to w scenowej kompozycji.
            Container.Bind<BattleRuntime>()
                .AsSingle()
                .NonLazy();

            Container.Bind<InventoryPanelPresentation>()
                .AsSingle()
                .NonLazy();

            Container.Bind<ItemDragService>()
                .AsSingle()
                .NonLazy();
        }

        private void bindUiSemanticsAndPrefabProviders() {
            // grid layout semantics
            Container.Bind<InventoryGridLayoutGroup>()
                .FromMethod(_ => new InventoryGridLayoutGroup(settings.inventoryGridLayout))
                .AsSingle()
                .NonLazy();

            Container.Bind<ItemsLayerRectTransform>()
                .FromMethod(_ => new ItemsLayerRectTransform(settings.itemsLayerRectTransform))
                .AsSingle()
                .NonLazy();

            // prefab providers
            Container.Bind<ItemViewPrefabItemView>()
                .FromMethod(_ => new ItemViewPrefabItemView(settings.placedItemViewPrefab))
                .AsSingle();

            Container.Bind<DragGhostPrefabItemView>()
                .FromMethod(_ => new DragGhostPrefabItemView(settings.dragGhostPrefab))
                .AsSingle();

            Container.Bind<GridViewPrefabInventoryGridView>()
                .FromInstance(new GridViewPrefabInventoryGridView(settings.gridViewPrefab))
                .AsSingle();

            Container.Bind<CellViewPrefabInventoryCellView>()
                .FromMethod(_ => new CellViewPrefabInventoryCellView(settings.cellViewPrefab))
                .AsSingle();

            // cell prefab itself (used by grid build)
            Container.Bind<InventoryCellView>()
                .FromInstance(settings.cellViewPrefab)
                .AsSingle();
        }

        private void bindUiPanelsAndPresenters() {
            Container.Bind<ICombatInventoryGridPanel>()
                .To<InventoryGridLayerContainer>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();

            Container.Bind<ICombatInventoryItemsPanel>()
                .To<InventoryItemsViewPresenter>()
                .AsSingle()
                .NonLazy();
        }

        private void bindUiFactories() {
            Container.Bind<CombatContextPresentationFactory>()
                .AsSingle();

            Container.Bind<IInventoryItemViewFactory>()
                .To<InventoryItemViewFactory>()
                .AsSingle();
        }

        private void bindUiEventHub() {
            Container.BindInterfacesTo<UiCombatContextEventHub>()
                .AsSingle();
        }

        private void bindBattleUi() {
            Container.Bind<CharacterPrefabAggregate>()
                .FromInstance(settings.battleSlotPrefab)
                .AsSingle();

            Container.Bind<Transform>()
                .WithId("BattleSlotParent")
                .FromInstance(settings.battleSlotParent)
                .AsSingle();

            // manager z hierarchii sceny
            Container.Bind<BattleUIManager>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}