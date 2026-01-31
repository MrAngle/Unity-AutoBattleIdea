using System;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using UnityEngine;

namespace Contracts.Character {
    public interface ICharacterAggregateFacade {
        public event Action<ICharacterAggregateFacade, long, long> OnHpChanged;
        public event Action<ICharacterAggregateFacade> OnDeath;

        bool TryEquipItem(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem);
        bool CanPlaceItem(IPlaceableItem item, Vector2Int origin);

        ICharacterInventoryFacade GetInventoryAggregate();

        public long GetMaxHp();

        public long GetCurrentHp();

        void Apply(DamageAmount damageAmount);

        string GetName();

        void Cleanup();
    }
}