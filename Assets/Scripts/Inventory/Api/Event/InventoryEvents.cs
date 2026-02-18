using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Api.Event {
    public interface IInventoryChangedEventListener {
        void OnEvent(in InventoryChanged ev);
    }

    public interface IInventoryItemPlacedEventListener {
        void OnEvent(in NewItemPlacedDtoEvent ev);
    }

    public readonly struct InventoryChanged : IInventoryDomainEvent {
        public readonly ICombatInventory combatInventory;

        public InventoryChanged(ICombatInventory combatInventory) {
            this.combatInventory = combatInventory;
        }
    }

    public readonly struct NewItemPlacedDtoEvent : IInventoryDomainEvent {
        public readonly long placedItemId;
        public readonly ShapeArchetype shapeArchetype;
        public readonly Vector2Int origin;

        public NewItemPlacedDtoEvent(
            long placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int origin) {
            this.placedItemId = placedItemId;
            this.shapeArchetype = shapeArchetype;
            this.origin = origin;
        }
    }
}