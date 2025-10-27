using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Items.Domain {
    public interface IPlacedItem {
        public IEnumerable<Vector2Int> getOccupiedCells();
        
        public static IPlacedItem CreateBattleItem(ItemData data, Vector2Int origin) {
            return new PlacedItem(data, origin);
        }
    }

    internal class PlacedItem : IPlacedItem
    {
        private ItemData Data { get; }
        private Vector2Int Origin { get; }

        internal PlacedItem(ItemData data, Vector2Int origin)
        {
            Data = data;
            Origin = origin;
        }

        public IEnumerable<Vector2Int> getOccupiedCells() {
            return Data.Shape.Cells.Select(offset => Origin + offset);
        }
    }
}