using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Controller.Api {
    public interface IPlaceableItem {
        IPlacedItem toPlacedItem(IGridInspector gridInspector /*do zastanowienia czy potrzebne*/, Vector2Int origin);
        ShapeArchetype getShape();
    }
}