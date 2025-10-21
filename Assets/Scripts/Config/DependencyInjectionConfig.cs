using Inventory.Slots;
using Inventory.Slots.Context;
using UnityEngine;
using Zenject;

namespace Config {
    public class DependencyInjectionConfig: MonoInstaller {
        // [SerializeField] private int width = 8;
        // [SerializeField] private int height = 6;

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
        }
    }
}