using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface ICharacterInventoryFacade {
        IEnumerable<IPlacedItem> GetPlacedSnapshot();
        IInventoryGrid GetInventoryGrid();
        public IPlacedItem Place(IPlaceableItem placeableItem, Vector2Int origin);
        public bool CanPlace(IPlaceableItem placeableItem, Vector2Int origin);
    }
}