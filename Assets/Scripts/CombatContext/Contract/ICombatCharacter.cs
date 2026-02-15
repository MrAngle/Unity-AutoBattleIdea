using System;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacter {
        public Id<CharacterId> getId();
        public event Action<ICombatCharacter, long, long> OnHpChanged;
        public event Action<ICombatCharacter> OnDeath;
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);
        bool canPlaceItem(EquipItemQuery equipItemQuery);
        ICombatCharacterInventory getInventoryAggregate();
        public long getMaxHp();
        public long getCurrentHp();
        void apply(PowerAmount powerAmount);
        string getName();
        void cleanup();
        void combatTick();
    }
}