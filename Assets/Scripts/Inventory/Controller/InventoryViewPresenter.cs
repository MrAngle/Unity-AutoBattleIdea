using System;
using System.Collections.Generic;
using MageFactory.Context;
using MageFactory.Item.Controller.Api;
using MageFactory.Semantics;
using MageFactory.Shared.Utility;
using UI.Popup;
using Zenject;
using Object = UnityEngine.Object;

namespace MageFactory.Inventory.Controller {
    public readonly struct ItemRemovedDtoEvent {
        public long PlacedItemId { get; }

        public ItemRemovedDtoEvent(long id) {
            PlacedItemId = id;
        }
    }

    public sealed class InventoryViewPresenter : IInitializable, IDisposable {
        private readonly InventoryAggregateContext _aggregateContext;

        private readonly IItemViewFactory _factory;
        // private readonly RectTransform _itemsLayerRectTransform;
        // private readonly GridLayoutGroup _inventoryGridLayout;

        private readonly SignalBus _signalBus;

        private readonly Dictionary<long, PlacedItemView> _views = new();

        [Inject]
        public InventoryViewPresenter(
            SignalBus signalBus,
            IItemViewFactory factory,
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup gridLayout,
            InventoryAggregateContext aggregateContext) {
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _factory = NullGuard.NotNullOrThrow(factory);
            _aggregateContext = NullGuard.NotNullOrThrow(aggregateContext);

            _aggregateContext.OnInventoryAggregateSet += PrintInventoryItems;
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

        private void Start() {
            RefreshView(); // to remove?
        }

        private void RefreshView() {
            PrintInventoryItems(_aggregateContext.getInventoryAggregateContext());
        }

        private void PrintInventoryItems(ICharacterInventoryFacade characterInventoryFacade) {
            Clear();
            foreach (var placedItem in characterInventoryFacade.getPlacedSnapshot()) {
                if (_views.ContainsKey(placedItem.getId())) continue;
                PlacedItemView view = _factory.Create(placedItem.getShape(), placedItem.getOrigin());
                _views[placedItem.getId()] = view;
            }
        }

        public void Clear() {
            foreach (var view in _views.Values)
                if (view != null)
                    Object.Destroy(view.gameObject);

            _views.Clear();
        }

        private void OnItemPlaced(ItemPlacedDtoEvent itemPlacedDtoEvent) {
            PlacedItemView view = _factory.Create(itemPlacedDtoEvent.Data, itemPlacedDtoEvent.Origin);
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