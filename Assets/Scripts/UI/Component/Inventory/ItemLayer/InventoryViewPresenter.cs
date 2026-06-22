using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Contract;
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
            public readonly bool isEntryPoint;
            public readonly FlowKind entryPointFlowKind;
            public readonly FlowPortKind flowPortKind;
            public readonly string flowPortName;
            public readonly string flowPortDescription;

            public NewItemPrintCommand(
                Id<ItemId> placedItemId,
                ShapeArchetype shapeArchetype,
                Vector2Int origin,
                bool isEntryPoint,
                FlowKind entryPointFlowKind)
                : this(placedItemId, shapeArchetype, origin, isEntryPoint, entryPointFlowKind,
                    FlowPortKind.None, string.Empty, string.Empty) {
            }

            public NewItemPrintCommand(
                Id<ItemId> placedItemId,
                ShapeArchetype shapeArchetype,
                Vector2Int origin,
                bool isEntryPoint,
                FlowKind entryPointFlowKind,
                FlowPortKind flowPortKind,
                string flowPortName,
                string flowPortDescription) {
                this.placedItemId = placedItemId;
                this.shapeArchetype = shapeArchetype;
                this.origin = origin;
                this.isEntryPoint = isEntryPoint;
                this.entryPointFlowKind = entryPointFlowKind;
                this.flowPortKind = flowPortKind;
                this.flowPortName = flowPortName ?? string.Empty;
                this.flowPortDescription = flowPortDescription ?? string.Empty;
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

        public readonly struct UiPrintPreparedGuardsCommand {
            public readonly IReadOnlyList<PreparedGuardState> guardStates;
            public readonly ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo;

            public UiPrintPreparedGuardsCommand(
                IReadOnlyList<PreparedGuardState> guardStates,
                ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
                this.guardStates = NullGuard.NotNullOrThrow(guardStates);
                this.inventoryGridInfo = inventoryGridInfo;
            }
        }

        public readonly struct UiShowGuardCreatedBeamCommand {
            public readonly Id<ItemId> sourceItemId;
            public readonly int sourceLocalRow;
            public readonly Id<GuardId> guardId;

            public UiShowGuardCreatedBeamCommand(
                Id<ItemId> sourceItemId,
                int sourceLocalRow,
                Id<GuardId> guardId) {
                this.sourceItemId = sourceItemId;
                this.sourceLocalRow = sourceLocalRow;
                this.guardId = guardId;
            }
        }

        public readonly struct UiShowAttackCreatedBeamCommand {
            public readonly Id<ItemId> sourceItemId;
            public readonly int sourceLocalRow;
            public readonly Vector3 targetWorldPosition;

            public UiShowAttackCreatedBeamCommand(
                Id<ItemId> sourceItemId,
                int sourceLocalRow,
                Vector3 targetWorldPosition) {
                this.sourceItemId = sourceItemId;
                this.sourceLocalRow = sourceLocalRow;
                this.targetWorldPosition = targetWorldPosition;
            }
        }

        public readonly struct UiShowFlowInputStartedCommand {
            public readonly Id<ItemId> inputItemId;

            public UiShowFlowInputStartedCommand(Id<ItemId> inputItemId) {
                this.inputItemId = inputItemId;
            }
        }

        public readonly struct UiShowFlowOutputReachedCommand {
            public readonly Id<ItemId> outputItemId;
            public readonly int outputLocalRow;
            public readonly long attackPower;
            public readonly long guardPower;

            public UiShowFlowOutputReachedCommand(
                Id<ItemId> outputItemId,
                int outputLocalRow,
                long attackPower,
                long guardPower) {
                this.outputItemId = outputItemId;
                this.outputLocalRow = outputLocalRow;
                this.attackPower = attackPower;
                this.guardPower = guardPower;
            }
        }

        public readonly struct UiShowFlowNoOutputCommand {
            public readonly Id<ItemId> finalItemId;
            public readonly int finalLocalRow;
            public readonly bool wasCommittedByLegacyRule;

            public UiShowFlowNoOutputCommand(
                Id<ItemId> finalItemId,
                int finalLocalRow,
                bool wasCommittedByLegacyRule) {
                this.finalItemId = finalItemId;
                this.finalLocalRow = finalLocalRow;
                this.wasCommittedByLegacyRule = wasCommittedByLegacyRule;
            }
        }

        public readonly struct UiShowGuardAbsorbedVisualCommand {
            public readonly Id<GuardId> guardId;
            public readonly long blockedDamage;

            public UiShowGuardAbsorbedVisualCommand(
                Id<GuardId> guardId,
                long blockedDamage) {
                this.guardId = guardId;
                this.blockedDamage = blockedDamage;
            }
        }

        public readonly struct UiShowGuardReplacedVisualCommand {
            public readonly Id<GuardId> guardId;
            public readonly long replacedGuardPower;

            public UiShowGuardReplacedVisualCommand(
                Id<GuardId> guardId,
                long replacedGuardPower) {
                this.guardId = guardId;
                this.replacedGuardPower = replacedGuardPower;
            }
        }

        public void printInventoryItems(UiPrintInventoryItemsCommand changeInventoryItemsCommand);
        public void printNewItem(NewItemPrintCommand command);
        public void printItemCastProgress(UiPrintItemCastProgressCommand command);
        public void printPreparedGuards(UiPrintPreparedGuardsCommand command);
        public void showFlowInputStarted(UiShowFlowInputStartedCommand command);
        public void showFlowOutputReached(UiShowFlowOutputReachedCommand command);
        public void showFlowNoOutput(UiShowFlowNoOutputCommand command);
        public void showGuardAbsorbedVisual(UiShowGuardAbsorbedVisualCommand command);
        public void showGuardReplacedVisual(UiShowGuardReplacedVisualCommand command);
        public void showGuardCreatedBeam(UiShowGuardCreatedBeamCommand command);
        public void showAttackCreatedBeam(UiShowAttackCreatedBeamCommand command);

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
        private PreparedGuardOverlayView preparedGuardOverlayView;
        private InventoryActionBeamOverlayView actionBeamOverlayView;
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
                applyEntryPointColor(view, placedItem);
                applyFlowPortVisual(view, placedItem);
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

            if (preparedGuardOverlayView != null) {
                Object.Destroy(preparedGuardOverlayView.gameObject);
                preparedGuardOverlayView = null;
            }

            if (actionBeamOverlayView != null) {
                Object.Destroy(actionBeamOverlayView.gameObject);
                actionBeamOverlayView = null;
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
            applyEntryPointColor(view, command);
            applyFlowPortVisual(view, command);
            view.setClickHandler(() => handleItemClicked(command.placedItemId));
            itemIdToItemView[command.placedItemId] = view;
            ensureFlowConnectionOverlayView(view.transform.parent);
            renderFocusState();
        }

        private static void applyEntryPointColor(PlacedItemView view, IGridItemPlaced placedItem) {
            if (placedItem is IInventoryPlacedEntryPoint entryPoint) {
                view.setColor(getEntryPointColor(entryPoint.getFlowKind()));
            }
        }

        private static void applyFlowPortVisual(PlacedItemView view, IGridItemPlaced placedItem) {
            if (placedItem is IFlowPortPlacedItem portPlacedItem) {
                view.setFlowPortVisual(
                    portPlacedItem.getFlowPortKind(),
                    portPlacedItem.getFlowPortName(),
                    portPlacedItem.getFlowPortDescription());
            }
        }

        private static void applyEntryPointColor(
            PlacedItemView view,
            ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            if (command.isEntryPoint) {
                view.setColor(getEntryPointColor(command.entryPointFlowKind));
            }
        }

        private static void applyFlowPortVisual(
            PlacedItemView view,
            ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            view.setFlowPortVisual(
                command.flowPortKind,
                command.flowPortName,
                command.flowPortDescription);
        }

        private static Color getEntryPointColor(FlowKind flowKind) {
            return flowKind == FlowKind.Defense
                ? new Color(0.28f, 0.82f, 0.62f, 0.9f)
                : new Color(1f, 0.48f, 0.28f, 0.9f);
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

        public void printPreparedGuards(ICombatInventoryItemsPanel.UiPrintPreparedGuardsCommand command) {
            ensurePreparedGuardOverlayView(getItemsLayerTransform());

            if (preparedGuardOverlayView == null) {
                return;
            }

            preparedGuardOverlayView.printGuards(command.guardStates, command.inventoryGridInfo);
        }

        public void showFlowInputStarted(ICombatInventoryItemsPanel.UiShowFlowInputStartedCommand command) {
            if (!itemIdToItemView.TryGetValue(command.inputItemId, out PlacedItemView itemView)) {
                return;
            }

            PopupManager.Instance?.Show(
                itemView,
                "IN",
                new Color(0.42f, 1f, 0.78f, 1f),
                moveY: 38f,
                duration: 0.55f);
        }

        public void showFlowOutputReached(ICombatInventoryItemsPanel.UiShowFlowOutputReachedCommand command) {
            if (!tryGetSourceItemPoint(command.outputItemId, command.outputLocalRow, out Vector2 point)) {
                return;
            }

            string label = command.guardPower > 0
                ? $"OUT G{GuardPowerLabelFormatter.format(command.guardPower)}"
                : $"OUT {GuardPowerLabelFormatter.format(command.attackPower)}";

            if (itemIdToItemView.TryGetValue(command.outputItemId, out PlacedItemView itemView)) {
                PopupManager.Instance?.Show(
                    itemView,
                    label,
                    new Color(0.46f, 0.72f, 1f, 1f),
                    moveY: 42f,
                    duration: 0.65f);
            }

            ensureActionBeamOverlayView(getItemsLayerTransform());
            actionBeamOverlayView?.showBeam(point, point + new Vector2(0f, 42f), new Color(0.46f, 0.72f, 1f, 1f));
        }

        public void showFlowNoOutput(ICombatInventoryItemsPanel.UiShowFlowNoOutputCommand command) {
            if (!tryGetSourceItemPoint(command.finalItemId, command.finalLocalRow, out Vector2 point)) {
                return;
            }

            string label = command.wasCommittedByLegacyRule
                ? "NO OUT"
                : "WASTED";

            if (itemIdToItemView.TryGetValue(command.finalItemId, out PlacedItemView itemView)) {
                PopupManager.Instance?.Show(
                    itemView,
                    label,
                    command.wasCommittedByLegacyRule
                        ? new Color(1f, 0.72f, 0.28f, 1f)
                        : new Color(1f, 0.22f, 0.22f, 1f),
                    moveY: 42f,
                    duration: 0.75f);
            }

            ensureActionBeamOverlayView(getItemsLayerTransform());
            actionBeamOverlayView?.showBeam(point, point + new Vector2(34f, 28f), new Color(1f, 0.28f, 0.22f, 1f));
        }

        public void showGuardAbsorbedVisual(ICombatInventoryItemsPanel.UiShowGuardAbsorbedVisualCommand command) {
            ensurePreparedGuardOverlayView(getItemsLayerTransform());
            preparedGuardOverlayView?.showGuardAbsorbedVisual(command.guardId, command.blockedDamage);
        }

        public void showGuardReplacedVisual(ICombatInventoryItemsPanel.UiShowGuardReplacedVisualCommand command) {
            ensurePreparedGuardOverlayView(getItemsLayerTransform());
            preparedGuardOverlayView?.showGuardReplacedVisual(command.guardId, command.replacedGuardPower);
        }

        public void showGuardCreatedBeam(ICombatInventoryItemsPanel.UiShowGuardCreatedBeamCommand command) {
            if (!tryGetSourceItemPoint(command.sourceItemId, command.sourceLocalRow, out Vector2 start)) {
                return;
            }

            ensurePreparedGuardOverlayView(getItemsLayerTransform());
            if (preparedGuardOverlayView == null) {
                return;
            }

            if (!preparedGuardOverlayView.tryGetGuardCenterInParent(command.guardId, out Vector2 end)) {
                end = preparedGuardOverlayView.getOverlayCenterInParent();
            }

            ensureActionBeamOverlayView(getItemsLayerTransform());
            actionBeamOverlayView?.showBeam(start, end, new Color(0.36f, 0.95f, 0.82f, 1f));
        }

        public void showAttackCreatedBeam(ICombatInventoryItemsPanel.UiShowAttackCreatedBeamCommand command) {
            if (!tryGetSourceItemPoint(command.sourceItemId, command.sourceLocalRow, out Vector2 start)) {
                return;
            }

            Transform itemsLayerTransform = getItemsLayerTransform();
            RectTransform itemsLayerRectTransform = itemsLayerTransform as RectTransform;
            if (itemsLayerRectTransform == null) {
                return;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, command.targetWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                itemsLayerRectTransform,
                screenPoint,
                null,
                out Vector2 end);

            ensureActionBeamOverlayView(itemsLayerTransform);
            actionBeamOverlayView?.showBeam(start, end, new Color(1f, 0.46f, 0.23f, 1f));
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

        private void ensurePreparedGuardOverlayView(Transform parent) {
            if (preparedGuardOverlayView != null || parent == null) {
                return;
            }

            preparedGuardOverlayView = PreparedGuardOverlayView.create(parent);
        }

        private void ensureActionBeamOverlayView(Transform parent) {
            if (actionBeamOverlayView != null || parent == null) {
                return;
            }

            actionBeamOverlayView = InventoryActionBeamOverlayView.create(parent);
        }

        private bool tryGetSourceItemPoint(
            Id<ItemId> itemId,
            int localRow,
            out Vector2 point) {
            if (itemIdToItemView.TryGetValue(itemId, out PlacedItemView itemView)) {
                if (itemView.tryGetRowCenterInParent(localRow, out point)) {
                    return true;
                }

                point = itemView.getCenterInParent();
                return true;
            }

            point = default;
            return false;
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