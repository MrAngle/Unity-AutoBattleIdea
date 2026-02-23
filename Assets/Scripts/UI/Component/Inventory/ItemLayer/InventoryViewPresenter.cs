using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using MageFactory.UI.Shared.Popup;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Controller {
    public interface ICombatInventoryItemsPanel {
        public readonly struct UiPrintInventoryItemsCommand {
            public readonly IEnumerable<IGridItemPlaced> characterEquippedItems;

            public UiPrintInventoryItemsCommand(IEnumerable<IGridItemPlaced> characterEquippedItems) {
                this.characterEquippedItems = characterEquippedItems;
            }
        }

        public readonly struct NewItemPrintCommand {
            public readonly long placedItemId;
            public readonly ShapeArchetype shapeArchetype;
            public readonly Vector2Int origin;

            public NewItemPrintCommand(
                long placedItemId,
                ShapeArchetype shapeArchetype,
                Vector2Int origin) {
                this.placedItemId = placedItemId;
                this.shapeArchetype = shapeArchetype;
                this.origin = origin;
            }
        }

        public void printInventoryItems(UiPrintInventoryItemsCommand changeInventoryItemsCommand);
        public void printNewItem(NewItemPrintCommand command);
    }


    internal sealed class InventoryItemsViewPresenter : IDisposable, /*IItemPlacedEventEventListener,*/
        ICombatInventoryItemsPanel {
        private readonly IInventoryItemViewFactory _factory;

        // private readonly IInventoryEventRegistry inventoryEventRegistry;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<long, PlacedItemView> _views = new();

        [Inject]
        internal InventoryItemsViewPresenter(
            SignalBus signalBus,
            IInventoryItemViewFactory factory
            /*IInventoryEventRegistry injectInventoryEventRegistry*/) {
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _factory = NullGuard.NotNullOrThrow(factory);
            // inventoryEventRegistry = NullGuard.NotNullOrThrow(injectInventoryEventRegistry);

            // inventoryEventRegistry.subscribe(this);
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
                if (_views.ContainsKey(placedItem.getId())) continue;
                PlacedItemView view = _factory.create(placedItem.getShape(), placedItem.getOrigin());
                _views[placedItem.getId()] = view;
            }
        }

        private void clear() {
            foreach (var view in _views.Values)
                if (view != null)
                    Object.Destroy(view.gameObject);

            _views.Clear();
        }

        private void OnItemRemoved(ItemRemovedDtoEvent itemRemovedEvent) {
            if (_views.TryGetValue(itemRemovedEvent.PlacedItemId, out var itemView)) {
                Object.Destroy(itemView.gameObject);
                _views.Remove(itemRemovedEvent.PlacedItemId);
            }
        }

        private void OnPowerChanged(ItemPowerChangedDtoEvent itemPowerChangedEvent) {
            if (_views.TryGetValue(itemPowerChangedEvent.ItemId, out var view))
                PopupManager.Instance.ShowHpChangeDamage(view, itemPowerChangedEvent.Delta);
        }

        public void printNewItem(ICombatInventoryItemsPanel.NewItemPrintCommand command) {
            PlacedItemView view = _factory.create(command.shapeArchetype, command.origin);
            _views[command.placedItemId] = view;
        }

        // public void onEvent(in NewItemPlacedDtoEvent ev) {
        //     PlacedItemView view = _factory.create(ev.shapeArchetype, ev.origin);
        //     _views[ev.placedItemId] = view;
        // }
    }
}