using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using Zenject;

namespace MageFactory.UI.Component.Inventory {
    public readonly struct UiPrintInventoryCommand {
        public readonly ICombatInventoryGridPanel.UiPrintInventoryGridCommand gridCommand;
        public readonly ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand itemsCommand;

        public UiPrintInventoryCommand(ICombatInventoryGridPanel.UiPrintInventoryGridCommand gridCommand,
                                       ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand itemsCommand) {
            this.gridCommand = gridCommand;
            this.itemsCommand = itemsCommand;
        }

        public static UiPrintInventoryCommand from(ICombatCharacterInventory combatCharacterInventory) {
            ICombatInventoryGridPanel.UiPrintInventoryGridCommand gridCommand =
                ICombatInventoryGridPanel.UiPrintInventoryGridCommand.from(combatCharacterInventory.getInventoryGrid());

            ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand itemsCommand =
                new(combatCharacterInventory.getPlacedSnapshot());

            return new UiPrintInventoryCommand(gridCommand, itemsCommand);
        }
    }

    public class InventoryPanelPresentation {
        private readonly ICombatInventoryGridPanel combatInventoryGridPanel;
        private readonly ICombatInventoryItemsPanel combatInventoryItemsPanel;

        [Inject]
        public InventoryPanelPresentation(ICombatInventoryGridPanel combatInventoryGridPanel,
                                          ICombatInventoryItemsPanel combatInventoryItemsPanel) {
            this.combatInventoryGridPanel = NullGuard.NotNullOrThrow(combatInventoryGridPanel);
            this.combatInventoryItemsPanel = NullGuard.NotNullOrThrow(combatInventoryItemsPanel);
        }

        public void printInventory(ICombatCharacterInventory combatCharacterInventory) {
            UiPrintInventoryCommand uiPrintInventoryCommand = UiPrintInventoryCommand.from(combatCharacterInventory);

            combatInventoryGridPanel.printInventoryGrid(uiPrintInventoryCommand.gridCommand);
            combatInventoryItemsPanel.printInventoryItems(uiPrintInventoryCommand.itemsCommand);
        }

        public void printNewItem(ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            combatInventoryItemsPanel.printNewItem(command);
        }
    }
}