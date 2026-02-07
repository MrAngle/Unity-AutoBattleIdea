using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface IPlacedItem : IInventoryPosition {
        public long GetId();
        public IActionDescription prepareItemActionDescription();
        public ShapeArchetype GetShape();
    }

    public interface IPlaceableItem {
        IPlacedItem ToPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype GetShape();
    }
}