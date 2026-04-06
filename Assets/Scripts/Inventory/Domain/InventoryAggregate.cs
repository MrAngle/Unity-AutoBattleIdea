using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryAggregate {
        private readonly HashSet<IInventoryPlacedEntryPoint> entryPoints = new();
        private readonly IInventoryEventPublisher inventoryEventHub;
        private readonly InventoryRegistry inventoryRegistry;
        private readonly IItemFactory itemFactory;

        private InventoryAggregate(InventoryRegistry inventoryRegistry,
                                   IItemFactory itemFactory,
                                   IInventoryEventPublisher inventoryEventPublisher) {
            NullGuard.NotNullCheckOrThrow(inventoryRegistry, itemFactory, inventoryEventPublisher);
            this.itemFactory = itemFactory;
            inventoryEventHub = inventoryEventPublisher;
            this.inventoryRegistry = inventoryRegistry;

            NullGuard.NotNullCheckOrThrow(this.inventoryRegistry, this.entryPoints, this.itemFactory,
                this.inventoryEventHub);
        }

        internal static InventoryAggregate create(IItemFactory itemFactory,
                                                  IInventoryEventPublisher inventoryEventPublisher) {
            InventoryRegistry registry = InventoryRegistry.createNew(17, 8);
            var aggregate = new InventoryAggregate(registry, itemFactory, inventoryEventPublisher);

            return aggregate;
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            return inventoryRegistry.getPlacedSnapshot();
        }

        public IReadOnlyInventoryGrid getInventoryGrid() {
            return new InventoryGrid(inventoryRegistry);
        }

        public bool canPlace(PlaceItemQuery placeItemQuery) {
            return inventoryRegistry.canPlaceItem(placeItemQuery.itemDefinition.getShape(), placeItemQuery.origin);
        }

        public IReadOnlyCollection<IInventoryPlacedEntryPoint> getEntryPointsToTick() {
            return inventoryRegistry.getEntryPoints();
        }

        public IInventoryPlacedItem place(PlaceItemCommand placeItemCommand) {
            // change to tryPlace
            if (!inventoryRegistry.canPlaceItem(placeItemCommand.itemDefinition.getShape(), placeItemCommand.origin))
                throw new ArgumentException("Cannot place item");

            CreatePlaceableItemCommand createPlaceableItemCommand = new(placeItemCommand.itemDefinition);
            var inventoryPlaceableItem = itemFactory.createPlacableItem(createPlaceableItemCommand);

            var inventoryPlacedItem =
                inventoryPlaceableItem.toPlacedItem(
                    InventoryPosition.create(placeItemCommand.origin,
                        placeItemCommand.itemDefinition.getShape().Shape));

            inventoryRegistry.placeItem(inventoryPlacedItem);

            if (inventoryPlacedItem is IInventoryPlacedEntryPoint entryPoint) {
                entryPoints.Add(entryPoint);
            }

            inventoryEventHub.publish(new NewItemPlacedDtoEvent(inventoryPlacedItem.getId(),
                inventoryPlacedItem.getShape(),
                placeItemCommand.origin));
            return inventoryPlacedItem;
        }

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<IInventoryPlacedItem> neighborItems) {
            var inventoryPlacedItems =
                inventoryRegistry.getNeighborItems(sourceGridItemPlaced, directions).ToList();

            if (inventoryPlacedItems.Count == 0) {
                neighborItems = Enumerable.Empty<IInventoryPlacedItem>();
                return false;
            }

            neighborItems = inventoryPlacedItems;
            return true;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem itemToReturn) {
            inventoryRegistry.tryGetItemAtCell(cell, out itemToReturn);

            return itemToReturn != null;
        }

        public bool tryChangeItemPosition(Id<ItemId> idOfItemToMove, Vector2Int newOriginPosition) {
            if (!inventoryRegistry.tryMoveItem(
                    idOfItemToMove,
                    newOriginPosition,
                    out var movedItem,
                    out var oldOriginPosition)) {
                return false;
            }

            inventoryEventHub.publish(new ItemPositionChangedDtoEvent(
                movedItem.getId(),
                newOriginPosition
            ));

            return true;
        }
    }
}