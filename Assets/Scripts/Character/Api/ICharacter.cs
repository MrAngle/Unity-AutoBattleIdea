using System;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using UnityEngine;

namespace MageFactory.Character.Api {
    public interface ICharacter {
        public event Action<ICharacter, long, long> OnHpChanged;
        public event Action<ICharacter> OnDeath;

        bool equipItemOrThrow(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem);
        bool canPlaceItem(IPlaceableItem item, Vector2Int origin);

        ICharacterInventoryFacade getInventoryAggregate();

        public long getMaxHp();

        public long getCurrentHp();

        void apply(DamageAmount damageAmount);

        string getName();

        void cleanup();
    }
}