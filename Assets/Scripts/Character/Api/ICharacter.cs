using System;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Api {
    public interface ICharacter {
        public event Action<ICharacter, long, long> OnHpChanged;
        public event Action<ICharacter> OnDeath;

        ICharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        // bool equipItemOrThrow(ICharacterEquipableItem item, Vector2Int origin, out ICharacterEquippedItem placedItem);
        bool canPlaceItem(EquipItemQuery equipItemQuery);
        // bool canPlaceItem(ICharacterEquipableItem item, Vector2Int origin);

        ICharacterInventory getInventoryAggregate();

        public long getMaxHp();

        public long getCurrentHp();

        void apply(PowerAmount powerAmount);

        string getName();

        void cleanup();
    }
}