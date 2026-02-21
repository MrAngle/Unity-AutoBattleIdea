using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Context;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Utility;
using UI.Popup;
using Zenject;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Controller {
    internal sealed class ViewPresenter : IInitializable, IDisposable, IItemPlacedEventEventListener {
        private readonly InventoryAggregateContext _aggregateContext;
        private readonly IInventoryItemViewFactory _factory;
        private readonly IInventoryEventRegistry inventoryEventRegistry;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<long, PlacedItemView> _views = new();

        [Inject]
        internal ViewPresenter(
            SignalBus signalBus,
            IInventoryItemViewFactory factory,
            InventoryAggregateContext aggregateContext,
            IInventoryEventRegistry injectInventoryEventRegistry) {
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _factory = NullGuard.NotNullOrThrow(factory);
            _aggregateContext = NullGuard.NotNullOrThrow(aggregateContext);
            inventoryEventRegistry = NullGuard.NotNullOrThrow(injectInventoryEventRegistry);

            _aggregateContext.OnInventoryAggregateSet += printInventoryItems;
        }

        public void Dispose() {
            _signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);

            inventoryEventRegistry.unsubscribe(this);
        }

        public void Initialize() {
            inventoryEventRegistry.subscribe(this);

            _signalBus.Subscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.Subscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        private void printInventoryItems(ICombatCharacterInventory characterInventoryFacade) {
            clear();
            foreach (var placedItem in characterInventoryFacade.getPlacedSnapshot()) {
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

        public void onEvent(in NewItemPlacedDtoEvent ev) {
            PlacedItemView view = _factory.create(ev.shapeArchetype, ev.origin);
            _views[ev.placedItemId] = view;
        }
    }
}