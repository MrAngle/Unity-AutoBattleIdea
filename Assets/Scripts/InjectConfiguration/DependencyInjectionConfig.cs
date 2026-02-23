using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Domain;
using MageFactory.BattleManager;
using MageFactory.Character.Contract;
using MageFactory.Character.Contract.Event;
using MageFactory.Character.Domain.Service;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Domain.Service;
using MageFactory.Flow.Api;
using MageFactory.Flow.Domain.Service;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Controller;
using MageFactory.Inventory.Domain.Service;
using MageFactory.Item.Domain.Service;
using MageFactory.Semantics;
using MageFactory.UI.Context.Combat;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Context.Combat.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.InjectConfiguration {
    public class DependencyInjectionConfig : MonoInstaller {
        [Header("Prefabs")] [SerializeField] [FormerlySerializedAs("itemViewPrefab")]
        private PlacedItemView placedItemViewPrefab;

        [SerializeField] private PlacedItemView dragGhostPrefab;
        [SerializeField] private InventoryGridView gridViewPrefab;
        [SerializeField] private InventoryCellView cellViewPrefab;

        [Header("RectTransforms")] [SerializeField]
        private RectTransform itemsLayerRectTransform;

        [Header("GridLayoutGroup")] [SerializeField]
        private GridLayoutGroup inventoryGridLayout;

        [Header("Battle UI")] [SerializeField] private CharacterPrefabAggregate battleSlotPrefab;

        [SerializeField] private Transform battleSlotParent;


        public override void InstallBindings() {
            installSignals();

            bindItemsLayerRectTransform();

            bindInventoryGridLayoutGroup();

            bindContexts();

            bindFactories();

            bindEventHandlers();
            bindUiEventHandlers();

            // PREFAB INITIALIZER
            Container.Bind<InventoryCellView>()
                .FromInstance(cellViewPrefab)
                .AsSingle();
            // Container.BindFactory<InventoryGridView, InventoryGridView.Factory>()
            //     .FromComponentInNewPrefab(gridViewPrefab)
            //     .AsSingle();

            Container.Bind<ItemViewPrefabItemView>()
                .FromMethod(_ => new ItemViewPrefabItemView(placedItemViewPrefab))
                .AsSingle();

            Container.Bind<DragGhostPrefabItemView>()
                .FromMethod(_ => new DragGhostPrefabItemView(dragGhostPrefab))
                .AsSingle();

            Container.Bind<GridViewPrefabInventoryGridView>()
                .FromInstance(new GridViewPrefabInventoryGridView(gridViewPrefab))
                .AsSingle();

            Container.Bind<CellViewPrefabInventoryCellView>()
                .FromMethod(_ => new CellViewPrefabInventoryCellView(cellViewPrefab))
                .AsSingle();

            Container.Bind<ICombatInventoryGridPanel>()
                .To<InventoryGridLayerContainer>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();

            Container.Bind<ICombatInventoryItemsPanel>()
                .To<InventoryItemsViewPresenter>()
                .AsSingle()
                .NonLazy();

            Container.Bind<ItemDragService>()
                .AsSingle()
                .NonLazy();

            Container.Bind<BattleRuntime>()
                .AsSingle()
                .NonLazy();

            Container.Bind<InventoryPanelPresentation>()
                .AsSingle()
                .NonLazy();
        }


        private void bindContexts() {
            Container.BindInterfacesAndSelfTo<CombatContextPresentationHandler>()
                .AsSingle()
                .NonLazy();
        }

        private void bindFactories() {
            Container.Bind<IFlowFactory>()
                .To<FlowFactoryService>()
                .AsSingle();

            Container.BindInterfacesTo<ItemFactoryService>()
                .AsSingle();

            Container.Bind<IInventoryFactory>()
                .To<InventoryFactoryService>()
                .AsSingle();
            Container.Bind<ICharacterCombatCapabilitiesFactory>()
                .To<CharacterCombatCapabilitiesFactoryService>()
                .AsSingle();
            Container.Bind<IInventoryItemViewFactory>()
                .To<InventoryItemViewFactory>()
                .AsSingle();

            Container.Bind<IActionExecutor>()
                .To<ActionExecutorService>()
                .AsSingle();

            Container.Bind<ICharacterFactory>()
                .To<CharacterFactoryService>()
                .AsSingle();

            Container.Bind<ICombatContextFactory>()
                .To<CombatContextFactoryService>()
                .AsSingle();

            bindCharactersAndBattleUI();
        }

        private void bindEventHandlers() {
            Container
                .Bind(
                    typeof(IInventoryEventPublisher),
                    typeof(IInventoryEventRegistry))
                .To<InventoryEventHub>()
                .AsSingle();

            Container
                .Bind(
                    typeof(ICombatContextEventPublisher),
                    typeof(ICombatContextEventRegistry))
                .To<CombatContextEventHub>()
                .AsSingle();
        }

        private void bindUiEventHandlers() {
            Container.BindInterfacesTo<UiCombatContextEventHub>()
                .AsSingle();
        }

        private void bindInventoryGridLayoutGroup() {
            Container.Bind<InventoryGridLayoutGroup>()
                .FromMethod(_ => new InventoryGridLayoutGroup(inventoryGridLayout))
                .AsSingle()
                .NonLazy();
        }

        private void bindItemsLayerRectTransform() {
            Container.Bind<ItemsLayerRectTransform>()
                .FromMethod(_ => new ItemsLayerRectTransform(itemsLayerRectTransform))
                .AsSingle()
                .NonLazy();
        }

        private void bindCharactersAndBattleUI() {
            // prefab slotu
            Container.Bind<CharacterPrefabAggregate>()
                .FromInstance(battleSlotPrefab)
                .AsSingle();

            // Container.Bind<CharacterPrefabAggregate>()
            //     .FromComponentInHierarchy()
            //     .AsSingle();

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

        private void installSignals() {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<ItemRemovedDtoEvent>();
            Container.DeclareSignal<ItemPowerChangedDtoEvent>();
        }
    }
}