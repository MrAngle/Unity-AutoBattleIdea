using Contracts.Inventory;
using Contracts.Items;
using UnityEngine;

namespace Inventory.Items.Domain {
    public class ItemArchetype : IPlaceableItem {
        private readonly ShapeArchetype _shapeArchetype;
        private readonly float _castTime = 0.05f; // for now

        public ItemArchetype(ShapeArchetype shapeArchetype) {
            _shapeArchetype = shapeArchetype;
        }

        public IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return new BattleItem(this, origin); // TODO
        }


        public ShapeArchetype GetShape() {
            return _shapeArchetype;
        }

        public float GetCastTime() {
            return _castTime;
        }
    }
}