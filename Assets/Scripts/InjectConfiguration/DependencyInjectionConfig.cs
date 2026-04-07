using MageFactory.UI.Component;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
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
            // DOMAIN (bez UI)
            MageFactoryDomainInstaller.Install(Container);

            // UI signals (DeclareSignal<T>())
            MageFactoryUiSignalsInstaller.Install(Container);

            // UNITY/UI
            MageFactoryUnityUiInstaller.Settings uiSettings = new MageFactoryUnityUiInstaller.Settings {
                placedItemViewPrefab = placedItemViewPrefab,
                dragGhostPrefab = dragGhostPrefab,
                gridViewPrefab = gridViewPrefab,
                cellViewPrefab = cellViewPrefab,
                itemsLayerRectTransform = itemsLayerRectTransform,
                inventoryGridLayout = inventoryGridLayout,
                battleSlotPrefab = battleSlotPrefab,
                battleSlotParent = battleSlotParent
            };

            MageFactoryUnityUiInstaller.Install(Container, uiSettings);
        }
    }
}