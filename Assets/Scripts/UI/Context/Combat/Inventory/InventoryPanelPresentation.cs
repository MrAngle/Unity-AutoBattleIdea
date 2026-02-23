using MageFactory.Inventory.Controller;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.UI.Context.Combat.Inventory {
    internal class InventoryPanelPresentation {
        private readonly ICombatInventoryGridPanel combatInventoryGridPanel;
        private readonly ICombatInventoryItemsPanel combatInventoryItemsPanel;

        [Inject]
        public InventoryPanelPresentation(ICombatInventoryGridPanel combatInventoryGridPanel,
                                          ICombatInventoryItemsPanel combatInventoryItemsPanel) {
            this.combatInventoryGridPanel = NullGuard.NotNullOrThrow(combatInventoryGridPanel);
            this.combatInventoryItemsPanel = NullGuard.NotNullOrThrow(combatInventoryItemsPanel);
        }
    }
}