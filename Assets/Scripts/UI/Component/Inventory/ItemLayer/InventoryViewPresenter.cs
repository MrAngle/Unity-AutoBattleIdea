using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Shared.Popup;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public interface ICombatInventoryItemsPanel {
        public readonly struct UiPrintInventoryItemsCommand {
            public readonly IEnumerable<IGridItemPlaced> characterEquippedItems;

            public UiPrintInventoryItemsCommand(IEnumerable<IGridItemPlaced> characterEquippedItems) {
                this.characterEquippedItems = characterEquippedItems;
            }
        }

        public readonly struct NewItemPrintCommand {
            public readonly Id<ItemId> placedItemId;
            public readonly ShapeArchetype shapeArchetype;
            public readonly Vector2Int origin;

            public NewItemPrintCommand(
                Id<ItemId> placedItemId,
                ShapeArchetype shapeArchetype,
                Vector2Int origin) {
                this.placedItemId = placedItemId;
                this.shapeArchetype = shapeArchetype;
                this.origin = origin;
            }
        }

        public readonly struct MoveItemToPositionCommand {
            public readonly Id<ItemId> placedItemId;
            public readonly Vector2Int newOrigin;

            public MoveItemToPositionCommand(
                Id<ItemId> placedItemId,
                Vector2Int newOrigin) {
                this.placedItemId = placedItemId;
                this.newOrigin = newOrigin;
            }
        }

        public readonly struct UiPrintItemCastProgressCommand {
            public readonly IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem;
            public readonly IReadOnlyList<FlowPathViewState> flowPaths;

            public UiPrintItemCastProgressCommand(
                IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem,
                IReadOnlyList<FlowPathViewState> flowPaths) {
                this.progressByItem = NullGuard.NotNullOrThrow(progressByItem);
                this.flowPaths = NullGuard.NotNullOrThrow(flowPaths);
            }
        }

        public void printInventoryItems(UiPrintInventoryItemsCommand changeInventoryItemsCommand);
        public void printNewItem(NewItemPrintCommand command);
        public void printItemCastProgress(UiPrintItemCastProgressCommand command);

        public void moveItemToPosition(MoveItemToPositionCommand command,
                                       ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo);
    }


    internal sealed class InventoryItemsViewPresenter : IDisposable, ICombatInventoryItemsPanel {
        private readonly IInventoryItemViewFactory inventoryItemViewFactory;

        private readonly SignalBus _signalBus;
        private readonly Dictionary<Id<ItemId>, PlacedItemView> itemIdToItemView = new();
        private readonly List<FlowPathViewState> latestFlowPaths = new();
        private readonly List<FlowPathViewState> visibleFlowPaths = new();
        private readonly Dictionary<Id<ItemId>, List<ItemCastProgressViewState>> focusedProgressByItem = new();

        private readonly Dictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> focusedProgressReadModel =
            new();

        private readonly HashSet<Id<ActiveFlowId>> visibleFlowIds = new();
        private readonly HashSet<Id<ItemId>> relatedItemIds = new();
        private readonly List<Id<ItemId>> focusedProgressItemIds = new();
        private FlowConnectionOverlayView flowConnectionOverlayView;
        private InventoryFocusKind focusKind = InventoryFocusKind.None;
        private Id<ItemId> focusedItemId;
        private Id<ActiveFlowId> focusedFlowId;

        [Inject]
        internal InventoryItemsViewPresenter(
            SignalBus signalBus,
            IInventoryItemViewFactory factory) {
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            inventoryItemViewFactory = NullGuard.NotNullOrThrow(factory);

            _signalBus.Subscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.Subscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        public void Dispose() {
            _signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);

            // inventoryEventRegistry.unsubscribe(this);
        }

        public void printInventoryItems(
            ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand changeInventoryItemsCommand) {
            clear();
            foreach (var placedItem in changeInventoryItemsCommand.characterEquippedItems) {
                if (itemIdToItemView.ContainsKey(placedItem.getId())) continue;
                PlacedItemView view = inventoryItemViewFactory.create(placedItem.getShape(), placedItem.getOrigin());
                Id<ItemId> placedItemId = placedItem.getId();
                view.setClickHandler(() => handleItemClicked(placedItemId));
                itemIdToItemView[placedItemId] = view;
                ensureFlowConnectionOverlayView(view.transform.parent);
            }

            renderFocusState();
        }

        private void clear() {
            foreach (var view in itemIdToItemView.Values)
                if (view != null)
                    Object.Destroy(view.gameObject);

            itemIdToItemView.Clear();
            latestFlowPaths.Clear();
            clearFocus();

            if (flowConnectionOverlayView != null) {
                Object.Destroy(flowConnectionOverlayView.gameObject);
                flowConnectionOverlayView = null;
            }
        }

        private void OnItemRemoved(ItemRemovedDtoEvent itemRemovedEvent) {
            if (itemIdToItemView.TryGetValue(itemRemovedEvent.PlacedItemId, out var itemView)) {
                Object.Destroy(itemView.gameObject);
                itemIdToItemView.Remove(itemRemovedEvent.PlacedItemId);
                if (focusKind == InventoryFocusKind.Item && focusedItemId == itemRemovedEvent.PlacedItemId) {
                    clearFocus();
                }
            }
        }

        private void OnPowerChanged(ItemPowerChangedDtoEvent itemPowerChangedEvent) {
            if (itemIdToItemView.TryGetValue(itemPowerChangedEvent.ItemId, out var view))
                PopupManager.Instance.ShowHpChangeDamage(view, itemPowerChangedEvent.Delta);
        }

        public void printNewItem(ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            PlacedItemView view = inventoryItemViewFactory.create(command.shapeArchetype, command.origin);
            view.setClickHandler(() => handleItemClicked(command.placedItemId));
            itemIdToItemView[command.placedItemId] = view;
            ensureFlowConnectionOverlayView(view.transform.parent);
            renderFocusState();
        }

        public void printItemCastProgress(ICombatInventoryItemsPanel.UiPrintItemCastProgressCommand command) {
            latestFlowPaths.Clear();
            latestFlowPaths.AddRange(command.flowPaths);

            IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem =
                getProgressByItemForCurrentFocus(command.progressByItem);

            foreach (KeyValuePair<Id<ItemId>, PlacedItemView> itemView in itemIdToItemView) {
                if (progressByItem.TryGetValue(
                        itemView.Key,
                        out IReadOnlyList<ItemCastProgressViewState> progressRatios)) {
                    itemView.Value.setCastProgressBars(progressRatios);
                }
                else {
                    itemView.Value.hideCastProgressBars();
                }
            }

            renderFocusState();
        }

        private void renderFocusState() {
            if (focusKind == InventoryFocusKind.None) {
                if (flowConnectionOverlayView != null) {
                    flowConnectionOverlayView.hideAll();
                }

                applyItemVisualStates(latestFlowPaths);
                return;
            }

            IReadOnlyList<FlowPathViewState> flowPathsToRender = getVisibleFlowPathsForCurrentFocus();

            if (flowPathsToRender.Count == 0 || itemIdToItemView.Count == 0) {
                if (flowConnectionOverlayView != null) {
                    flowConnectionOverlayView.hideAll();
                }
            }
            else {
                ensureFlowConnectionOverlayView(getItemsLayerTransform());
                flowConnectionOverlayView.printConnections(flowPathsToRender, itemIdToItemView, handleFlowClicked);
            }

            applyItemVisualStates(flowPathsToRender);
        }

        private IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>>
            getProgressByItemForCurrentFocus(
                IReadOnlyDictionary<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> progressByItem) {
            if (focusKind == InventoryFocusKind.None) {
                return progressByItem;
            }

            IReadOnlyList<FlowPathViewState> flowPathsToRender = getVisibleFlowPathsForCurrentFocus();
            if (focusKind == InventoryFocusKind.None) {
                return progressByItem;
            }

            rebuildVisibleFlowIds(flowPathsToRender);
            clearFocusedProgressBuffers();

            foreach (KeyValuePair<Id<ItemId>, IReadOnlyList<ItemCastProgressViewState>> itemProgress in
                     progressByItem) {
                List<ItemCastProgressViewState> filteredProgress = null;

                for (int i = 0; i < itemProgress.Value.Count; i++) {
                    ItemCastProgressViewState progress = itemProgress.Value[i];

                    if (!visibleFlowIds.Contains(progress.getFlowId())) {
                        continue;
                    }

                    if (filteredProgress == null) {
                        filteredProgress = new List<ItemCastProgressViewState>();
                        focusedProgressByItem[itemProgress.Key] = filteredProgress;
                        focusedProgressReadModel[itemProgress.Key] = filteredProgress;
                        focusedProgressItemIds.Add(itemProgress.Key);
                    }

                    filteredProgress.Add(progress);
                }
            }

            return focusedProgressReadModel;
        }

        private void clearFocusedProgressBuffers() {
            for (int i = 0; i < focusedProgressItemIds.Count; i++) {
                Id<ItemId> itemId = focusedProgressItemIds[i];
                focusedProgressByItem[itemId].Clear();
            }

            focusedProgressItemIds.Clear();
            focusedProgressReadModel.Clear();
        }

        private IReadOnlyList<FlowPathViewState> getVisibleFlowPathsForCurrentFocus() {
            visibleFlowPaths.Clear();

            switch (focusKind) {
                case InventoryFocusKind.Item:
                    appendItemFocusedFlowPaths();
                    return visibleFlowPaths;
                case InventoryFocusKind.Flow:
                    appendSingleFocusedFlowPath();
                    return visibleFlowPaths;
                default:
                    return latestFlowPaths;
            }
        }

        private void appendItemFocusedFlowPaths() {
            bool entryPointFocus = hasFlowStartedBy(focusedItemId);

            for (int i = 0; i < latestFlowPaths.Count; i++) {
                FlowPathViewState flowPath = latestFlowPaths[i];

                if (entryPointFocus) {
                    if (isFlowStartedBy(flowPath, focusedItemId)) {
                        visibleFlowPaths.Add(flowPath);
                    }

                    continue;
                }

                if (tryCreateLocalItemContextPath(flowPath, focusedItemId, out FlowPathViewState localPath)) {
                    visibleFlowPaths.Add(localPath);
                }
            }
        }

        private void appendSingleFocusedFlowPath() {
            for (int i = 0; i < latestFlowPaths.Count; i++) {
                FlowPathViewState flowPath = latestFlowPaths[i];

                if (flowPath.getFlowId() == focusedFlowId) {
                    visibleFlowPaths.Add(flowPath);
                    return;
                }
            }

            clearFocus();
        }

        private bool hasFlowStartedBy(Id<ItemId> itemId) {
            for (int i = 0; i < latestFlowPaths.Count; i++) {
                if (isFlowStartedBy(latestFlowPaths[i], itemId)) {
                    return true;
                }
            }

            return false;
        }

        private static bool isFlowStartedBy(FlowPathViewState flowPath, Id<ItemId> itemId) {
            IReadOnlyList<ItemFlowProcessingSlot> processingPath = flowPath.getProcessingPath();
            return processingPath.Count > 0 && processingPath[0].getItemId() == itemId;
        }

        private static bool tryCreateLocalItemContextPath(
            FlowPathViewState flowPath,
            Id<ItemId> itemId,
            out FlowPathViewState localPath) {
            IReadOnlyList<ItemFlowProcessingSlot> processingPath = flowPath.getProcessingPath();
            int itemPathIndex = findItemPathIndex(processingPath, itemId);

            if (itemPathIndex < 0) {
                localPath = default;
                return false;
            }

            int firstIndex = Mathf.Max(0, itemPathIndex - 1);
            int lastIndex = Mathf.Min(processingPath.Count - 1, itemPathIndex + 1);
            var localProcessingPath = new List<ItemFlowProcessingSlot>();

            for (int i = firstIndex; i <= lastIndex; i++) {
                localProcessingPath.Add(processingPath[i]);
            }

            localPath = new FlowPathViewState(
                flowPath.getFlowId(),
                flowPath.getFlowVisualIndex(),
                localProcessingPath,
                flowPath.getCurrentProgressRatio());
            return true;
        }

        private static int findItemPathIndex(IReadOnlyList<ItemFlowProcessingSlot> processingPath, Id<ItemId> itemId) {
            for (int i = 0; i < processingPath.Count; i++) {
                if (processingPath[i].getItemId() == itemId) {
                    return i;
                }
            }

            return -1;
        }

        private void applyItemVisualStates(IReadOnlyList<FlowPathViewState> flowPathsToRender) {
            if (focusKind == InventoryFocusKind.None) {
                foreach (KeyValuePair<Id<ItemId>, PlacedItemView> itemView in itemIdToItemView) {
                    itemView.Value.setVisualState(InventoryItemVisualState.Normal);
                }

                return;
            }

            rebuildRelatedItemIds(flowPathsToRender);

            foreach (KeyValuePair<Id<ItemId>, PlacedItemView> itemView in itemIdToItemView) {
                InventoryItemVisualState visualState = resolveVisualStateFor(itemView.Key);
                itemView.Value.setVisualState(visualState);
            }
        }

        private InventoryItemVisualState resolveVisualStateFor(Id<ItemId> itemId) {
            if (focusKind == InventoryFocusKind.Item && focusedItemId == itemId) {
                return InventoryItemVisualState.Focused;
            }

            if (relatedItemIds.Contains(itemId)) {
                return InventoryItemVisualState.Related;
            }

            return InventoryItemVisualState.Dimmed;
        }

        private void rebuildRelatedItemIds(IReadOnlyList<FlowPathViewState> flowPathsToRender) {
            relatedItemIds.Clear();

            for (int i = 0; i < flowPathsToRender.Count; i++) {
                IReadOnlyList<ItemFlowProcessingSlot> processingPath = flowPathsToRender[i].getProcessingPath();

                for (int pathIndex = 0; pathIndex < processingPath.Count; pathIndex++) {
                    relatedItemIds.Add(processingPath[pathIndex].getItemId());
                }
            }
        }

        private void rebuildVisibleFlowIds(IReadOnlyList<FlowPathViewState> flowPathsToRender) {
            visibleFlowIds.Clear();

            for (int i = 0; i < flowPathsToRender.Count; i++) {
                visibleFlowIds.Add(flowPathsToRender[i].getFlowId());
            }
        }

        private void handleItemClicked(Id<ItemId> itemId) {
            if (focusKind == InventoryFocusKind.Item && focusedItemId == itemId) {
                clearFocus();
            }
            else {
                focusKind = InventoryFocusKind.Item;
                focusedItemId = itemId;
                focusedFlowId = default;
            }

            renderFocusState();
        }

        private void handleFlowClicked(Id<ActiveFlowId> flowId) {
            if (focusKind == InventoryFocusKind.Flow && focusedFlowId == flowId) {
                clearFocus();
            }
            else {
                focusKind = InventoryFocusKind.Flow;
                focusedFlowId = flowId;
                focusedItemId = default;
            }

            renderFocusState();
        }

        private void clearFocus() {
            focusKind = InventoryFocusKind.None;
            focusedItemId = default;
            focusedFlowId = default;
        }

        public void moveItemToPosition(ICombatInventoryItemsPanel.MoveItemToPositionCommand command,
                                       ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            if (!itemIdToItemView.TryGetValue(command.placedItemId, out var existingView)) {
                Debug.LogWarning($"Brak widoku dla itemu {command.placedItemId}. Ruch pominięty.");
                return;
            }

            Vector2 targetAnchoredPosition = calculateAnchoredPosition(command.newOrigin, inventoryGridInfo);
            existingView.animateMoveTo(targetAnchoredPosition);
        }

        private Vector2 calculateAnchoredPosition(Vector2Int origin,
                                                  ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            float x = origin.x * (inventoryGridInfo.CellSize.x + inventoryGridInfo.Spacing.x);
            float y = -origin.y * (inventoryGridInfo.CellSize.y + inventoryGridInfo.Spacing.y);

            return inventoryGridInfo.GridOrigin + new Vector2(x, y);
        }

        private void ensureFlowConnectionOverlayView(Transform parent) {
            if (flowConnectionOverlayView != null || parent == null) {
                return;
            }

            flowConnectionOverlayView = FlowConnectionOverlayView.create(parent);
        }

        private Transform getItemsLayerTransform() {
            foreach (KeyValuePair<Id<ItemId>, PlacedItemView> itemView in itemIdToItemView) {
                if (itemView.Value != null) {
                    return itemView.Value.transform.parent;
                }
            }

            return null;
        }

        private enum InventoryFocusKind {
            None,
            Item,
            Flow
        }
    }
}