using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Context;
using MageFactory.Shared.Utility;
using UI.Popup;
using Zenject;
// using MageFactory.Inventory.Api;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Controller {
    internal sealed class InventoryViewPresenter : IInitializable, IDisposable {
        private readonly InventoryAggregateContext _aggregateContext;
        private readonly IInventoryItemViewFactory _factory;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<long, PlacedItemView> _views = new();

        [Inject]
        internal InventoryViewPresenter(
            SignalBus signalBus,
            IInventoryItemViewFactory factory,
            InventoryAggregateContext aggregateContext) {
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _factory = NullGuard.NotNullOrThrow(factory);
            _aggregateContext = NullGuard.NotNullOrThrow(aggregateContext);

            _aggregateContext.OnInventoryAggregateSet += printInventoryItems;
        }

        public void Dispose() {
            _signalBus.TryUnsubscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            _signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        public void Initialize() {
            _signalBus.Subscribe<ItemPlacedDtoEvent>(OnItemPlaced);
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

        private void OnItemPlaced(ItemPlacedDtoEvent itemPlacedDtoEvent) {
            PlacedItemView view = _factory.create(itemPlacedDtoEvent.Data, itemPlacedDtoEvent.Origin);
            _views[itemPlacedDtoEvent.PlacedItemId] = view;
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
    }
}