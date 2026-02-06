using System;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
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

        void apply(PowerAmount powerAmount);

        string getName();

        void cleanup();
    }
}