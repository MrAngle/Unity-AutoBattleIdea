using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model.Shape;

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

        public IInventoryPlacedItem toPlacedItem(
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities
        ) {
            return new BattleItem(this, inventoryPosition);
        }

        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        internal float getCastTime() {
            return castTime;
        }
    }
}