using MageFactory.Inventory.Api;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item {
    public class ItemArchetype : IPlaceableItem {
        private readonly float castTime = 0.05f; // for now
        private readonly ShapeArchetype _shapeArchetype;

        public ItemArchetype(ShapeArchetype shapeArchetype) {
            _shapeArchetype = shapeArchetype;
        }

        public IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return new BattleItem(this, origin); // TODO
        }


        public ShapeArchetype GetShape() {
            return _shapeArchetype;
        }

        public float getCastTime() {
            return castTime;
        }
    }
}