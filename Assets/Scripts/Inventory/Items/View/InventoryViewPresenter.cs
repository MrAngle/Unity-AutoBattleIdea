using System;
using System.Collections.Generic;
using Config.Semantics;
using Inventory.Items.Domain;
using UI.Popup;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Inventory.Items.View {
    
    public readonly struct ItemPowerChangedDtoEvent
    {
        public long ItemId { get; }
        public long Delta { get; }
        public ItemPowerChangedDtoEvent(long id, long delta)
        { ItemId = id; Delta = delta; }
    }
    
    public readonly struct ItemPlacedDtoEvent
    {
        public long PlacedItemId { get; }
        public ShapeArchetype Data { get; }
        public Vector2Int Origin { get; }

        public ItemPlacedDtoEvent(long id, ShapeArchetype data, Vector2Int origin)
        {
            PlacedItemId = id; Data = data; Origin = origin;
        }
    }

    public readonly struct ItemRemovedDtoEvent
    {
        public long PlacedItemId { get; }
        public ItemRemovedDtoEvent(long id) { PlacedItemId = id; }
    }

    public sealed class InventoryViewPresenter : IInitializable, IDisposable
    {
        // private readonly RectTransform _itemsLayerRectTransform;
        // private readonly GridLayoutGroup _inventoryGridLayout;
        
        private readonly SignalBus _signalBus;
        private readonly IItemViewFactory _factory;
        private readonly InventoryAggregateContext _aggregateContext;

        private readonly Dictionary<long, ItemView> _views = new();

        [Inject]
        public InventoryViewPresenter(
            SignalBus signalBus,
            IItemViewFactory factory,
            ItemsLayerRectTransform itemsLayer,
            InventoryGridLayoutGroup gridLayout,
            InventoryAggregateContext aggregateContext)
        {
            _signalBus = signalBus;
            _factory = factory;
            _aggregateContext = aggregateContext;
            // _itemsLayerRectTransform = itemsLayer.Get();
            // _inventoryGridLayout = gridLayout.Get();
        }

        public void Initialize()
        {
            _signalBus.Subscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            _signalBus.Subscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.Subscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
            
            RefreshView();
        }

        private void RefreshView() {
            // TODO
            // Its possible that some items are already placed in the aggregate
            // but not yet in the view.
            // Thats not true that is "refresh" but "re-create"
            // But its solution for now, remember to fix it later and prepare a proper diff update
            var agg = _aggregateContext.GetInventoryAggregate();
            foreach (IPlacedItem placedItem in agg.GetPlacedSnapshot())
            {
                if (_views.ContainsKey(placedItem.GetId())) {
                    continue;
                }
                ItemView view = _factory.Create(placedItem.GetShape(), placedItem.GetOrigin());
                _views[placedItem.GetId()] = view;
            }
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<ItemPlacedDtoEvent>(OnItemPlaced);
            _signalBus.TryUnsubscribe<ItemRemovedDtoEvent>(OnItemRemoved);
            _signalBus.TryUnsubscribe<ItemPowerChangedDtoEvent>(OnPowerChanged);
        }

        private void OnItemPlaced(ItemPlacedDtoEvent itemPlacedDtoEvent)
        {
            ItemView view = _factory.Create(itemPlacedDtoEvent.Data, itemPlacedDtoEvent.Origin);
            _views[itemPlacedDtoEvent.PlacedItemId] = view;
        }

        private void OnItemRemoved(ItemRemovedDtoEvent itemRemovedEvent)
        {
            if (_views.TryGetValue(itemRemovedEvent.PlacedItemId, out var itemView))
            {
                Object.Destroy(itemView.gameObject);
                _views.Remove(itemRemovedEvent.PlacedItemId);
            }
        }
        
        private void OnPowerChanged(ItemPowerChangedDtoEvent itemPowerChangedEvent) {
            if (_views.TryGetValue(itemPowerChangedEvent.ItemId, out var view)) {
                PopupManager.Instance.ShowHpChangeDamage(view, itemPowerChangedEvent.Delta);
            }
        }
    }

}