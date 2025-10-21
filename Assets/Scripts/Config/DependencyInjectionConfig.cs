using Config.Semantics;
using Inventory.Items.View;
using Inventory.Slots;
using Inventory.Slots.Context;
using UnityEngine;
using Zenject;

namespace Config {
    public class DependencyInjectionConfig: MonoInstaller {
        // [SerializeField] private int width = 8;

        [Header("Prefabs")]
        [SerializeField] private ItemView itemViewPrefab;
        [SerializeField] private ItemView dragGhostPrefab;
        
        [Header("RectTransforms")]
        [SerializeField] private RectTransform itemsLayerRectTransform;

        

        public override void InstallBindings()
        {
            // Jeden model siatki na scenę (lub kontekst)
            // Container.Bind<InventoryGrid>()
            //     .FromMethod(_ => new InventoryGrid(width, height))
            //     .AsSingle()
            //     .NonLazy();            
            //
            Container.Bind<InventoryGridContext>()
                .FromMethod(_ => InventoryGridContext.Create())
                .AsSingle()
                .NonLazy();
            
            Container.Bind<ItemsLayerRectTransform>()
                .FromMethod(_ => new ItemsLayerRectTransform(itemsLayerRectTransform))
                .AsSingle()
                .NonLazy();

            Container.Bind<ItemViewPrefabItemView>()
                .FromMethod(_ => new ItemViewPrefabItemView(itemViewPrefab))
                .AsSingle();
            
            Container.Bind<DragGhostPrefabItemView>()
                .FromMethod(_ => new DragGhostPrefabItemView(dragGhostPrefab))
                .AsSingle();
            
            // Container.BindFactory<ItemViewPrefabItemView, ItemView.Factory>()
            //     .FromComponentInNewPrefab(itemViewPrefab)
            //     .UnderTransform(itemsLayerRectTransform) // od razu podpinamy do ItemsLayer
            //     .AsTransient();
            
            // Container.BindFactory<ItemView, ItemView.Factory>()
            //     .FromComponentInNewPrefab(dragGhostPrefab)
            //     .UnderTransform(itemsLayerRectTransform)
            //     .AsTransient();
        }
    }
}