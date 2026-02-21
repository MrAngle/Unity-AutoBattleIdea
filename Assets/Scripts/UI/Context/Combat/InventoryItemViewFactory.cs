using MageFactory.Semantics;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.Inventory.Controller {
    internal sealed class InventoryItemViewFactory : IInventoryItemViewFactory {
        private readonly GridLayoutGroup _grid;
        private readonly ItemsLayerRectTransform _itemsLayer;
        private readonly PlacedItemView _prefab;

        [Inject]
        public InventoryItemViewFactory(
            ItemViewPrefabItemView prefabProvider,
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup gridLayoutGroup) {
            _prefab = NullGuard.NotNullOrThrow(prefabProvider.Get());
            _itemsLayer = NullGuard.NotNullOrThrow(itemsLayer);
            _grid = NullGuard.NotNullOrThrow(gridLayoutGroup.Get());
        }

        public PlacedItemView create(ShapeArchetype data, Vector2Int origin) {
            PlacedItemView placedItemView = Object.Instantiate(_prefab, _itemsLayer.Get(), false);

            Vector2 cell = _grid.cellSize;
            Vector2 spacing = _grid.spacing;

            placedItemView.build(data, cell);
            placedItemView.setOriginInGrid(origin, cell, Vector2.zero, spacing.x);

            return placedItemView;
        }
    }
}