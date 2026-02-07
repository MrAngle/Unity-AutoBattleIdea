using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Controller {
    internal interface IInventoryItemViewFactory {
        PlacedItemView create(ShapeArchetype data, Vector2Int origin);
    }
}