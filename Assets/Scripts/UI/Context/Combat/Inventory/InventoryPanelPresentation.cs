using MageFactory.CombatContext.Contract;
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

        public void setInventory(ICombatCharacterInventory combatCharacterInventory) {
            ICombatInventory combatInventory = combatCharacterInventory.getInventoryGrid();
            ICombatInventory invReferenceCopy = combatInventory;

            ICombatInventoryGridPanel.UiPrintInventoryGridCommand printInventoryGridCommand =
                new ICombatInventoryGridPanel.UiPrintInventoryGridCommand(
                    invReferenceCopy.Width,
                    invReferenceCopy.Height,
                    coord => invReferenceCopy.getState(coord));
            combatInventoryGridPanel.printInventoryGrid(printInventoryGridCommand);

            ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand itemsCommand =
                new(combatCharacterInventory.getPlacedSnapshot());
            combatInventoryItemsPanel.printInventoryItems(itemsCommand);
        }
    }
}