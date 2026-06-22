using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.InventoryItems;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;

namespace MageFactory.Item.Domain {
    internal class ItemArchetype : IInventoryPlaceableItem {
        private readonly IItemDefinition itemDefinition;

        private ItemArchetype(IItemDefinition itemDefinition) {
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

        public FlowPortKind getFlowPortKind() {
            return itemDefinition is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortKind()
                : FlowPortKind.None;
        }

        public string getFlowPortName() {
            return itemDefinition is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortName()
                : string.Empty;
        }

        public string getFlowPortDescription() {
            return itemDefinition is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortDescription()
                : string.Empty;
        }

        public IActionDescription getActionDescription() {
            return itemDefinition.getActionDescription();
        }

        public ShapeArchetype getShape() {
            return itemDefinition.getShape();
        }
    }
}