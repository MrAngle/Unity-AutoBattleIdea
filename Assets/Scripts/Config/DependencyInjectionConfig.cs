using Config.Semantics;
using Inventory.Slots;
using Inventory.Slots.Context;
using UnityEngine;
using Zenject;

namespace Config {
    public class DependencyInjectionConfig: MonoInstaller {
        // [SerializeField] private int width = 8;

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
        }
    }
}