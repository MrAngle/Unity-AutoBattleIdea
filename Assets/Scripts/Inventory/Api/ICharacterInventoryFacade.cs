using System.Collections.Generic;
using UnityEngine;

namespace MageFactory.Inventory.Api {
    public interface ICharacterInventoryFacade {
        IEnumerable<IPlacedItem> getPlacedSnapshot();
        IInventoryGrid getInventoryGrid();
        public IPlacedItem place(IPlaceableItem placeableItem, Vector2Int origin);
        public bool canPlace(IPlaceableItem placeableItem, Vector2Int origin);
    }
}