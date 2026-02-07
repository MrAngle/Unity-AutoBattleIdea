using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Domain {
    internal class ItemArchetype : IInventoryPlaceableItem {
        private readonly float castTime = 0.05f; // for now
        private readonly ShapeArchetype shapeArchetype;

        private ItemArchetype(ShapeArchetype shapeArchetype) {
            this.shapeArchetype = shapeArchetype;
        }

        internal static IInventoryPlaceableItem create(IItemDefinition itemDefinition) {
            return new ItemArchetype(itemDefinition.getShape());
        }

        public IInventoryPlacedItem toPlacedItem(IInventoryInspector gridInspector, Vector2Int origin) {
            return new BattleItem(this, origin); // TODO
        }

        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        internal float getCastTime() {
            return castTime;
        }
    }
}