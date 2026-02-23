using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    internal interface IInventoryItemViewFactory {
        PlacedItemView create(ShapeArchetype data, Vector2Int origin);
    }
}