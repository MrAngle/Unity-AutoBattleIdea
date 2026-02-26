using System;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain {
    internal class CombatCharacter : ICombatCharacter {
        private readonly CharacterAggregate characterAggregate;

        public Id<CharacterId> getId() {
            throw new NotImplementedException();
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            throw new NotImplementedException();
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            throw new NotImplementedException();
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            throw new NotImplementedException();
        }

        public long getMaxHp() {
            throw new NotImplementedException();
        }

        public long getCurrentHp() {
            throw new NotImplementedException();
        }

        public void apply(PowerAmount powerAmount) {
            throw new NotImplementedException();
        }

        public string getName() {
            throw new NotImplementedException();
        }

        public void cleanup() {
            throw new NotImplementedException();
        }

        public void combatTick(IFlowConsumer flowConsumer) {
            throw new NotImplementedException();
        }

        public ICharacterCombatCapabilities getCharacterCombatCapabilities() {
            throw new NotImplementedException();
        }

        public Team getTeam() {
            throw new NotImplementedException();
        }
    }
}