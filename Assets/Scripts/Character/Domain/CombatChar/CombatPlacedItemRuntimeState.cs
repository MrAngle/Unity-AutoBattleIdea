using System;
using System.Collections.Generic;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal sealed class CombatPlacedItemRuntimeState {
        private readonly Id<ItemId> itemId;
        private readonly List<ProcessingSlotState> processingSlots = new();

        internal CombatPlacedItemRuntimeState(IFlowItem item) {
            IFlowItem flowItem = NullGuard.NotNullOrThrow(item);
            itemId = NullGuard.ValidIdOrThrow(flowItem.getId());
            buildProcessingSlotsFromShape(flowItem);
        }

        internal Id<ItemId> getItemId() {
            return itemId;
        }

        internal bool canAcceptFlow() {
            for (int i = 0; i < processingSlots.Count; i++) {
                if (processingSlots[i].canAcceptFlow()) {
                    return true;
                }
            }

            return false;
        }

        internal ItemFlowProcessingSlot startProcessingFlow() {
            for (int i = 0; i < processingSlots.Count; i++) {
                if (processingSlots[i].canAcceptFlow()) {
                    return processingSlots[i].startProcessingFlow();
                }
            }

            throw new InvalidOperationException(
                $"Item '{itemId}' cannot accept more flows. " +
                $"Active flows: {getActiveFlowCount()}, capacity: {processingSlots.Count}.");
        }

        internal void finishProcessingFlow(ItemFlowProcessingSlot processingSlot) {
            ItemFlowProcessingSlot slot = NullGuard.NotNullOrThrow(processingSlot);

            if (!slot.getItemId().Equals(itemId)) {
                throw new InvalidOperationException(
                    $"Cannot finish processing item '{itemId}' with slot owned by item '{slot.getItemId()}'.");
            }

            for (int i = 0; i < processingSlots.Count; i++) {
                if (processingSlots[i].matches(slot)) {
                    processingSlots[i].finishProcessingFlow();
                    return;
                }
            }

            throw new InvalidOperationException(
                $"Cannot finish processing item '{itemId}' because row '{slot.getLocalRow()}' is not a processing slot.");
        }

        internal int getActiveFlowCount() {
            int activeFlowCount = 0;

            for (int i = 0; i < processingSlots.Count; i++) {
                if (processingSlots[i].isProcessingFlow()) {
                    activeFlowCount++;
                }
            }

            return activeFlowCount;
        }

        private void buildProcessingSlotsFromShape(IFlowItem flowItem) {
            var cellCountByLocalRow = new Dictionary<int, int>();

            foreach (Vector2Int cell in flowItem.getShape().Shape.Cells) {
                if (!cellCountByLocalRow.ContainsKey(cell.y)) {
                    cellCountByLocalRow[cell.y] = 0;
                }

                cellCountByLocalRow[cell.y]++;
            }

            var localRows = new List<int>(cellCountByLocalRow.Keys);
            localRows.Sort();

            for (int i = 0; i < localRows.Count; i++) {
                int localRow = localRows[i];
                processingSlots.Add(new ProcessingSlotState(
                    ItemFlowProcessingSlot.of(
                        itemId,
                        localRow,
                        cellCountByLocalRow[localRow])));
            }
        }

        private sealed class ProcessingSlotState {
            private readonly ItemFlowProcessingSlot processingSlot;
            private bool processingFlow;

            internal ProcessingSlotState(ItemFlowProcessingSlot processingSlot) {
                this.processingSlot = NullGuard.NotNullOrThrow(processingSlot);
            }

            internal bool canAcceptFlow() {
                return !processingFlow;
            }

            internal ItemFlowProcessingSlot startProcessingFlow() {
                if (processingFlow) {
                    throw new InvalidOperationException(
                        $"Processing slot row '{processingSlot.getLocalRow()}' is already processing a flow.");
                }

                processingFlow = true;
                return processingSlot;
            }

            internal void finishProcessingFlow() {
                if (!processingFlow) {
                    throw new InvalidOperationException(
                        $"Processing slot row '{processingSlot.getLocalRow()}' has no active flow to finish.");
                }

                processingFlow = false;
            }

            internal bool isProcessingFlow() {
                return processingFlow;
            }

            internal bool matches(ItemFlowProcessingSlot slot) {
                return processingSlot.Equals(slot);
            }
        }
    }
}