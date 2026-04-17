using System;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatEvents;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        private CharacterAggregate characterAggregate;

        public CombatCommandBus(CharacterAggregate characterAggregate) {
            this.characterAggregate = characterAggregate;
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return new CombatCharacterEquippedItem(characterAggregate.equipItemOrThrow(item));
        }

        public void takeDamage(DamageToReceive powerAmount) {
            characterAggregate.takeDamage(powerAmount);
        }

        public void processCombatEvent(CombatEvent combatEvent) {
            throw new NotImplementedException();
        }

        internal bool tryMoveItemToRight(ICharacterEquippedItem characterEquippedItem) {
            return characterAggregate.tryMoveItem(characterEquippedItem);
        }
    }
}