#nullable enable
using System;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate {
        private readonly Id<CharacterId> characterId;
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        private CharacterAggregate(
            CharacterData data,
            ICharacterInventory characterInventoryFacade,
            ICharacterEventPublisher characterEventPublisher
        ) {
            characterId = new Id<CharacterId>(IdGenerator.Next());
            characterData = NullGuard.NotNullOrThrow(data);
            characterInventory = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.characterEventPublisher = NullGuard.NotNullOrThrow(characterEventPublisher);

            characterData.OnHpChanged += handleCharacterDataHpChanged;
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

        public Id<CharacterId> getId() {
            return characterId;
        }

        public Id<CharacterId> getFlowOwnerCharacterId() {
            return getId();
        }

        public string getName() {
            return characterData.getName();
        }

        public ICharacterInventory getInventoryAggregate() {
            return characterInventory;
        }

        public long getMaxHp() {
            return characterData.getMaxHp();
        }

        public long getCurrentHp() {
            return characterData.CurrentHp;
        }

        public void apply(PowerAmount powerAmount) {
            characterData.applyDamage(powerAmount);
            if (characterData.CurrentHp <= 0) {
                characterEventPublisher.publish(new CharacterDeathDtoEvent(characterId));
            }
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterInventory.canPlace(new PlaceItemQuery(equipItemQuery.itemDefinition,
                equipItemQuery.origin));
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand equipItemCommand) {
            if (!canPlaceItem(new EquipItemQuery(equipItemCommand.itemDefinition, equipItemCommand.origin))) {
                throw new ArgumentException("Cannot equip item");
            }

            return characterInventory.place(
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin));
        }

        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        private void handleCharacterDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            characterEventPublisher.publish(new CharacterHpChangedDtoEvent(characterId, newHp, previousHpValue));
        }
    }
}