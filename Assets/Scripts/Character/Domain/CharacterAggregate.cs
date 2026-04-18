using System;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.Service;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate {
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        private CharacterAggregate(
            CharacterData data,
            ICharacterInventory characterInventoryFacade,
            ICharacterEventPublisher characterEventPublisher
        ) {
            characterData = NullGuard.NotNullOrThrow(data);
            characterInventory = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.characterEventPublisher = NullGuard.NotNullOrThrow(characterEventPublisher);

            characterData.onHpChanged += handleCharacterDataHpChanged;
        }

        public static CharacterAggregate createFrom(
            CreateCombatCharacterCommand characterCreateCommand,
            ICharacterInventory characterInventoryFacade, // I think it will be better when a character creates inventory
            ICharacterEventPublisher characterEventPublisher
        ) {
            return new CharacterAggregate(
                CharacterData.from(characterCreateCommand),
                characterInventoryFacade,
                characterEventPublisher);
        }

        public IReadOnlyCharacterData getCharacterInfo() {
            return characterData;
        }

        public ICharacterInventory getInventoryAggregate() {
            return characterInventory;
        }

        public DamageTaken takeDamage(ResolvedDamage resolvedDamage) {
            DamageTaken damageTaken = characterData.takeDamage(resolvedDamage);
            if (characterData.getCurrentHp() <= 0) {
                characterEventPublisher.publish(new CharacterDeathDtoEvent(characterData.getCharacterId()));
            }

            return damageTaken;
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterInventory.canPlace(new PlaceItemQuery(equipItemQuery.itemDefinition,
                equipItemQuery.origin));
        }

        public ICharacterEquippedItem equipItemOrThrow(EquipItemCommand equipItemCommand) {
            if (!canPlaceItem(new EquipItemQuery(equipItemCommand.itemDefinition, equipItemCommand.origin))) {
                throw new ArgumentException("Cannot equip item");
            }

            return characterInventory.place(
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin));
        }

        public void cleanup() {
            characterData.onHpChanged -= handleCharacterDataHpChanged;
        }

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            characterData.onHpChanged -= handleCharacterDataHpChanged;
        }

        private void handleCharacterDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            characterEventPublisher.publish(new CharacterHpChangedDtoEvent(characterData.getCharacterId(), newHp,
                previousHpValue));
        }

        public bool tryMoveItem(ICharacterEquippedItem itemToMove) {
            return characterInventory.tryMoveItem(itemToMove, itemToMove.getOrigin() + Vector2Int.right);
        }
    }
}