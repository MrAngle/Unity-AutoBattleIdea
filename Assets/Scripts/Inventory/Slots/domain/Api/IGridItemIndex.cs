// Assets/Scripts/Inventory/Slots/IGridItemIndex.cs
using System.Collections.Generic;
using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.Slots.Domain.Api
{
    /// Zapewnia mapowania: kratka -> item (+origin), oraz item -> zajęte kratki.
    public interface IGridItemIndex
    {
        bool TryGetItemAtCell(Vector2Int cell, out ShapeArchetype item, out Vector2Int origin);
        IEnumerable<Vector2Int> GetOccupiedCells(ShapeArchetype item, Vector2Int origin);

        // Utrzymanie indeksu (wołaj wraz z InventoryGrid.Place/Remove)
        void Register(ShapeArchetype item, Vector2Int origin);
        void Unregister(ShapeArchetype item);
        bool Contains(ShapeArchetype item);
    }
}