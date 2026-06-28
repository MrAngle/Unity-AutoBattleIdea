using System;
using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using UnityEngine;
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
        private readonly PreparedGuardPrintBuffer preparedGuardPrintBuffer = new();

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
            characterCombatQueries.collectActiveFlowStates(itemCastProgressPrintBuffer);

            combatInventoryItemsPanel.printItemCastProgress(
                new ICombatInventoryItemsPanel.UiPrintItemCastProgressCommand(
                    itemCastProgressPrintBuffer.getProgressByItem(),
                    itemCastProgressPrintBuffer.getFlowPaths()));
        }

        public void printPreparedGuards(ICharacterCombatQueries characterCombatQueries) {
            NullGuard.NotNullOrThrow(characterCombatQueries);

            preparedGuardPrintBuffer.clear();
            characterCombatQueries.collectPreparedGuardStates(preparedGuardPrintBuffer);

            combatInventoryItemsPanel.printPreparedGuards(
                new ICombatInventoryItemsPanel.UiPrintPreparedGuardsCommand(
                    preparedGuardPrintBuffer.getGuardStates(),
                    combatInventoryGridPanel.getInventoryGridInfo()));
        }

        public void printDefenseLayers(ICharacterCombatQueries characterCombatQueries) {
            ICharacterCombatQueries validQueries = NullGuard.NotNullOrThrow(characterCombatQueries);
            IReadOnlyCombatCharacterData characterInfo = validQueries.getCharacterInfo();

            combatInventoryItemsPanel.printDefenseLayers(
                new ICombatInventoryItemsPanel.UiPrintDefenseLayersCommand(
                    validQueries.getCurrentStability(),
                    validQueries.getBaselineStability(),
                    characterInfo.getCurrentHp(),
                    characterInfo.getMaxHp(),
                    combatInventoryGridPanel.getInventoryGridInfo()));
        }

        public void showFlowInputStarted(Id<ItemId> inputItemId) {
            combatInventoryItemsPanel.showFlowInputStarted(
                new ICombatInventoryItemsPanel.UiShowFlowInputStartedCommand(inputItemId));
        }

        public void showFlowOutputReached(
            Id<ItemId> outputItemId,
            int outputLocalRow,
            long attackPower,
            long guardPower) {
            combatInventoryItemsPanel.showFlowOutputReached(
                new ICombatInventoryItemsPanel.UiShowFlowOutputReachedCommand(
                    outputItemId,
                    outputLocalRow,
                    attackPower,
                    guardPower,
                    0));
        }

        public void showFlowOutputReached(
            Id<ItemId> outputItemId,
            int outputLocalRow,
            long attackPower,
            long guardPower,
            long stabilityPower) {
            combatInventoryItemsPanel.showFlowOutputReached(
                new ICombatInventoryItemsPanel.UiShowFlowOutputReachedCommand(
                    outputItemId,
                    outputLocalRow,
                    attackPower,
                    guardPower,
                    stabilityPower));
        }

        public void showFlowNoOutput(
            Id<ItemId> finalItemId,
            int finalLocalRow,
            bool wasCommittedByLegacyRule) {
            combatInventoryItemsPanel.showFlowNoOutput(
                new ICombatInventoryItemsPanel.UiShowFlowNoOutputCommand(
                    finalItemId,
                    finalLocalRow,
                    wasCommittedByLegacyRule));
        }

        public void showGuardAbsorbedVisual(
            Id<GuardId> guardId,
            long blockedDamage) {
            combatInventoryItemsPanel.showGuardAbsorbedVisual(
                new ICombatInventoryItemsPanel.UiShowGuardAbsorbedVisualCommand(
                    guardId,
                    blockedDamage));
        }

        public void showGuardReplacedVisual(
            Id<GuardId> guardId,
            long replacedGuardPower) {
            combatInventoryItemsPanel.showGuardReplacedVisual(
                new ICombatInventoryItemsPanel.UiShowGuardReplacedVisualCommand(
                    guardId,
                    replacedGuardPower));
        }

        public void showGuardCreatedBeam(
            Id<ItemId> sourceItemId,
            int sourceLocalRow,
            Id<GuardId> guardId) {
            combatInventoryItemsPanel.showGuardCreatedBeam(
                new ICombatInventoryItemsPanel.UiShowGuardCreatedBeamCommand(
                    sourceItemId,
                    sourceLocalRow,
                    guardId));
        }

        public void showStabilityGeneratedBeam(
            Id<ItemId> sourceItemId,
            int sourceLocalRow,
            long stabilityPower) {
            combatInventoryItemsPanel.showStabilityGeneratedBeam(
                new ICombatInventoryItemsPanel.UiShowStabilityGeneratedBeamCommand(
                    sourceItemId,
                    sourceLocalRow,
                    stabilityPower));
        }

        public void showStabilityAbsorbedVisual(
            long reducedDamage,
            long stabilityStrain,
            long remainingDamage) {
            combatInventoryItemsPanel.showStabilityAbsorbedVisual(
                new ICombatInventoryItemsPanel.UiShowStabilityAbsorbedVisualCommand(
                    reducedDamage,
                    stabilityStrain,
                    remainingDamage));
        }

        public void showHpChangedVisual(long hpDelta) {
            combatInventoryItemsPanel.showHpChangedVisual(
                new ICombatInventoryItemsPanel.UiShowHpChangedVisualCommand(hpDelta));
        }

        public void showDamagePacketLayer(
            long packetId,
            int layerIndex,
            long damageValue,
            bool completesPacket) {
            combatInventoryItemsPanel.showDamagePacketLayer(
                new ICombatInventoryItemsPanel.UiShowDamagePacketLayerCommand(
                    packetId,
                    layerIndex,
                    damageValue,
                    completesPacket));
        }

        public void showAttackCreatedBeam(
            Id<ItemId> sourceItemId,
            int sourceLocalRow,
            Vector3 targetWorldPosition) {
            combatInventoryItemsPanel.showAttackCreatedBeam(
                new ICombatInventoryItemsPanel.UiShowAttackCreatedBeamCommand(
                    sourceItemId,
                    sourceLocalRow,
                    targetWorldPosition));
        }

        public void moveItemToPosition(ICombatInventoryItemsPanel.MoveItemToPositionCommand command) {
            ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo =
                combatInventoryGridPanel.getInventoryGridInfo();
            combatInventoryItemsPanel.moveItemToPosition(command, inventoryGridInfo);
        }

        private sealed class ItemCastProgressPrintBuffer : IActiveFlowStateCollector {
            private readonly Dictionary<Id<ItemId>, List<ItemCastProgressViewState>> mutableProgressByItem = new();
            private readonly Dictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem = new();
            private readonly List<Id<ItemId>> itemIdsWithProgress = new();
            private readonly List<FlowPathViewState> flowPaths = new();
            private readonly FlowVisualIndexAllocator visualIndexAllocator = new();

            internal void clear() {
                for (int i = 0; i < itemIdsWithProgress.Count; i++) {
                    Id<ItemId> itemId = itemIdsWithProgress[i];
                    mutableProgressByItem[itemId].Clear();
                }

                itemIdsWithProgress.Clear();
                progressByItem.Clear();
                flowPaths.Clear();
                visualIndexAllocator.beginRefresh();
            }

            public void addActiveFlowState(ActiveFlowState flowState) {
                ActiveFlowCastState castState = flowState.getCastState();
                Id<ItemId> itemId = castState.getItemId();
                NullGuard.ValidIdOrThrow(itemId);
                int flowVisualIndex = visualIndexAllocator.getVisualIndex(flowState.getFlowId());

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
                    flowState.getFlowId(),
                    castState.getProcessingSlot().getLocalRow(),
                    calculateProgressRatio(castState),
                    flowVisualIndex));

                flowPaths.Add(new FlowPathViewState(
                    flowState.getFlowId(),
                    flowVisualIndex,
                    flowState.getProcessingPath(),
                    calculateProgressRatio(castState)));
            }

            internal IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> getProgressByItem() {
                visualIndexAllocator.endRefresh();
                return progressByItem;
            }

            internal IReadOnlyList<FlowPathViewState> getFlowPaths() {
                return flowPaths;
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

        private sealed class PreparedGuardPrintBuffer : IPreparedGuardStateCollector {
            private readonly List<PreparedGuardState> guardStates = new();

            internal void clear() {
                guardStates.Clear();
            }

            public void addPreparedGuardState(PreparedGuardState guardState) {
                guardStates.Add(guardState);
            }

            internal IReadOnlyList<PreparedGuardState> getGuardStates() {
                return guardStates;
            }
        }

        private sealed class FlowVisualIndexAllocator {
            private const int AvailableVisualIndexes = 8;

            private readonly Dictionary<Id<ActiveFlowId>, int> visualIndexByFlowId = new();
            private readonly HashSet<Id<ActiveFlowId>> seenFlowIds = new();
            private readonly int[] activeUsageByIndex = new int[AvailableVisualIndexes];
            private readonly List<Id<ActiveFlowId>> flowIdsToRelease = new();

            internal void beginRefresh() {
                seenFlowIds.Clear();
            }

            internal int getVisualIndex(Id<ActiveFlowId> flowId) {
                Id<ActiveFlowId> validFlowId = NullGuard.ValidIdOrThrow(flowId);
                seenFlowIds.Add(validFlowId);

                if (visualIndexByFlowId.TryGetValue(validFlowId, out int existingVisualIndex)) {
                    return existingVisualIndex;
                }

                int visualIndex = reserveVisualIndex();
                visualIndexByFlowId[validFlowId] = visualIndex;
                return visualIndex;
            }

            internal void endRefresh() {
                flowIdsToRelease.Clear();

                foreach (Id<ActiveFlowId> flowId in visualIndexByFlowId.Keys) {
                    if (!seenFlowIds.Contains(flowId)) {
                        flowIdsToRelease.Add(flowId);
                    }
                }

                for (int i = 0; i < flowIdsToRelease.Count; i++) {
                    Id<ActiveFlowId> flowId = flowIdsToRelease[i];
                    int visualIndex = visualIndexByFlowId[flowId];

                    visualIndexByFlowId.Remove(flowId);
                    activeUsageByIndex[visualIndex]--;
                }
            }

            private int reserveVisualIndex() {
                int leastUsedIndex = 0;
                int leastUsedCount = activeUsageByIndex[0];

                for (int i = 0; i < activeUsageByIndex.Length; i++) {
                    if (activeUsageByIndex[i] == 0) {
                        activeUsageByIndex[i]++;
                        return i;
                    }

                    if (activeUsageByIndex[i] < leastUsedCount) {
                        leastUsedCount = activeUsageByIndex[i];
                        leastUsedIndex = i;
                    }
                }

                activeUsageByIndex[leastUsedIndex]++;
                return leastUsedIndex;
            }
        }
    }
}