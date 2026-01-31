using System.Collections.Generic;
using Contracts.Items;
using UnityEngine;

namespace Contracts.Inventory {
    public interface ICharacterInventoryFacade {
        IEnumerable<IPlacedItem> GetPlacedSnapshot();
        IInventoryGrid GetInventoryGrid();
        public IPlacedItem Place(IPlaceableItem placeableItem, Vector2Int origin);
        public bool CanPlace(IPlaceableItem placeableItem, Vector2Int origin);
    }
}