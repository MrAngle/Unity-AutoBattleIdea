using MageFactory.Semantics;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal sealed class InventoryItemViewFactory : IInventoryItemViewFactory {
        private readonly GridLayoutGroup gridLayoutGroup;
        private readonly ItemsLayerRectTransform itemsLayerRectTransform;
        private readonly PlacedItemView placedItemView;

        [Inject]
        public InventoryItemViewFactory(
            ItemViewPrefabItemView prefabProvider,
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup gridLayoutGroup) {
            placedItemView = NullGuard.NotNullOrThrow(prefabProvider.Get());
            itemsLayerRectTransform = NullGuard.NotNullOrThrow(itemsLayer);
            this.gridLayoutGroup = NullGuard.NotNullOrThrow(gridLayoutGroup.Get());
        }

        public PlacedItemView create(ShapeArchetype data, Vector2Int origin) {
            PlacedItemView createdPlacedItemView =
                Object.Instantiate(placedItemView, itemsLayerRectTransform.Get(), false);

            Vector2 cell = gridLayoutGroup.cellSize;
            Vector2 spacing = gridLayoutGroup.spacing;

            createdPlacedItemView.build(data, cell);
            createdPlacedItemView.setOriginInGrid(origin, cell, Vector2.zero, spacing.x);

            return createdPlacedItemView;
        }
    }
}