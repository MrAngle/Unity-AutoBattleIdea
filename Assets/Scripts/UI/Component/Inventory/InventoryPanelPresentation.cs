using System;
using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
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
        private readonly ItemCastProgressPrintBuffer itemCastProgressPrintBuffer = new();

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

        public void printItemCastProgress(ICharacterCombatQueries characterCombatQueries) {
            NullGuard.NotNullOrThrow(characterCombatQueries);

            itemCastProgressPrintBuffer.clear();
            characterCombatQueries.collectActiveFlowCastStates(itemCastProgressPrintBuffer);

            combatInventoryItemsPanel.printItemCastProgress(
                new ICombatInventoryItemsPanel.UiPrintItemCastProgressCommand(
                    itemCastProgressPrintBuffer.getProgressByItem()));
        }

        public void moveItemToPosition(ICombatInventoryItemsPanel.MoveItemToPositionCommand command) {
            ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo =
                combatInventoryGridPanel.getInventoryGridInfo();
            combatInventoryItemsPanel.moveItemToPosition(command, inventoryGridInfo);
        }

        private sealed class ItemCastProgressPrintBuffer : IActiveFlowCastStateCollector {
            private readonly Dictionary<Id<ItemId>, List<ItemCastProgressViewState>> mutableProgressByItem = new();
            private readonly Dictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem = new();
            private readonly List<Id<ItemId>> itemIdsWithProgress = new();

            internal void clear() {
                for (int i = 0; i < itemIdsWithProgress.Count; i++) {
                    Id<ItemId> itemId = itemIdsWithProgress[i];
                    mutableProgressByItem[itemId].Clear();
                }

                itemIdsWithProgress.Clear();
                progressByItem.Clear();
            }

            public void addActiveFlowCastState(ActiveFlowCastState castState) {
                Id<ItemId> itemId = castState.getItemId();
                NullGuard.ValidIdOrThrow(itemId);

                if (!mutableProgressByItem.TryGetValue(
                        itemId,
                        out List<ItemCastProgressViewState> itemProgressBars)) {
                    itemProgressBars = new List<ItemCastProgressViewState>();
                    mutableProgressByItem[itemId] = itemProgressBars;
                }

                if (itemProgressBars.Count == 0) {
                    itemIdsWithProgress.Add(itemId);
                    progressByItem[itemId] = itemProgressBars;
                }

                itemProgressBars.Add(new ItemCastProgressViewState(
                    castState.getProcessingSlot().getLocalRow(),
                    calculateProgressRatio(castState)));
            }

            internal IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> getProgressByItem() {
                return progressByItem;
            }

            private static float calculateProgressRatio(ActiveFlowCastState castState) {
                int requiredTicks = castState.getRequiredCastTicks().getValue();

                if (requiredTicks <= 0) {
                    return 1f;
                }

                int remainingTicks = Math.Max(0, castState.getRemainingCastTicks().getValue());
                int completedTicks = Math.Max(0, requiredTicks - remainingTicks);
                return (float)completedTicks / requiredTicks;
            }
        }
    }
}