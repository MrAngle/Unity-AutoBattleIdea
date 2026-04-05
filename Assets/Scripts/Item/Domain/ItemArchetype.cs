using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.InventoryItems;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;

namespace MageFactory.Item.Domain {
    internal class ItemArchetype : IInventoryPlaceableItem {
        // private readonly float castTime = 0.05f; // for now
        // private readonly ShapeArchetype shapeArchetype;
        private readonly IItemDefinition itemDefinition;

        private ItemArchetype(IItemDefinition itemDefinition) {
            // this.shapeArchetype = NullGuard.NotNullOrThrow(shapeArchetype);
            this.itemDefinition = NullGuard.NotNullOrThrow(itemDefinition);
        }

        internal static IInventoryPlaceableItem create(IItemDefinition itemDefinition) {
            return new ItemArchetype(itemDefinition);
        }

        public IInventoryPlacedItem toPlacedItem(IInventoryPosition inventoryPosition) {
            return new InventoryPlacedBattleItem(new BattleItem(this, inventoryPosition));
        }

        public IItemDefinition getItemDefinition() {
            return itemDefinition;
        }

        public IActionDescription getActionDescription() {
            return itemDefinition.getActionDescription();
        }

        public ShapeArchetype getShape() {
            return itemDefinition.getShape();
        }

        // internal Duration getCastTime() {
        //     return itemDefinition.getActionDescription().getCastTime();
        // }
    }
}