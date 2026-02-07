using MageFactory.Semantics;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public interface IItemViewFactory {
        PlacedItemView Create(ShapeArchetype data, Vector2Int origin);
    }

    public sealed class ItemViewFactory : IItemViewFactory {
        private readonly GridLayoutGroup _grid;
        private readonly ItemsLayerRectTransform _itemsLayer;
        private readonly PlacedItemView _prefab;

        [Inject]
        public ItemViewFactory(
            ItemViewPrefabItemView prefabProvider,
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup gridLayoutGroup) {
            _prefab = prefabProvider.Get();
            _itemsLayer = itemsLayer;
            _grid = gridLayoutGroup.Get();
        }

        public PlacedItemView Create(ShapeArchetype data, Vector2Int origin) {
            var view = Object.Instantiate(_prefab, _itemsLayer.Get(), false);

            var cell = _grid.cellSize;
            var spacing = _grid.spacing;

            view.Build(data, cell);
            view.SetOriginInGrid(origin, cell, Vector2.zero, spacing.x);

            return view;
        }
    }
}