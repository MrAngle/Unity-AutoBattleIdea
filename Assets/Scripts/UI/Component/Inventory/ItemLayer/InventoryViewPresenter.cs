using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
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

        public void printInventoryItems(UiPrintInventoryItemsCommand changeInventoryItemsCommand);
        public void printNewItem(NewItemPrintCommand command);

        public void moveItemToPosition(MoveItemToPositionCommand command,
                                       ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo);
    }


    internal sealed class InventoryItemsViewPresenter : IDisposable, ICombatInventoryItemsPanel {
        private readonly IInventoryItemViewFactory inventoryItemViewFactory;

        private readonly SignalBus _signalBus;
        private readonly Dictionary<Id<ItemId>, PlacedItemView> itemIdToItemView = new();

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
                itemIdToItemView[placedItem.getId()] = view;
            }
        }

        private void clear() {
            foreach (var view in itemIdToItemView.Values)
                if (view != null)
                    Object.Destroy(view.gameObject);

            itemIdToItemView.Clear();
        }

        private void OnItemRemoved(ItemRemovedDtoEvent itemRemovedEvent) {
            if (itemIdToItemView.TryGetValue(itemRemovedEvent.PlacedItemId, out var itemView)) {
                Object.Destroy(itemView.gameObject);
                itemIdToItemView.Remove(itemRemovedEvent.PlacedItemId);
            }
        }

        private void OnPowerChanged(ItemPowerChangedDtoEvent itemPowerChangedEvent) {
            if (itemIdToItemView.TryGetValue(itemPowerChangedEvent.ItemId, out var view))
                PopupManager.Instance.ShowHpChangeDamage(view, itemPowerChangedEvent.Delta);
        }

        public void printNewItem(ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            PlacedItemView view = inventoryItemViewFactory.create(command.shapeArchetype, command.origin);
            itemIdToItemView[command.placedItemId] = view;
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
    }
}