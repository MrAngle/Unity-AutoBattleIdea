using System;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate : ICharacter {
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventoryFacade;
        private readonly Team team;

        private CharacterAggregate(CharacterData data, ICharacterInventory characterInventoryFacade, Team team) {
            characterData = NullGuard.NotNullOrThrow(data);
            this.characterInventoryFacade = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.team = NullGuard.enumDefinedOrThrow(team);

            // Subskrybuj event z danych, by przekazywać go dalej
            characterData.OnHpChanged += handleCharacterDataHpChanged;
        }

        public string getName() {
            return characterData.getName();
        }

        public ICharacterInventory getInventoryAggregate() {
            return characterInventoryFacade;
        }

        public long getMaxHp() {
            return characterData.getMaxHp();
        }

        public long getCurrentHp() {
            return characterData.CurrentHp;
        }

        public event Action<ICharacter, long, long> OnHpChanged;
        public event Action<ICharacter> OnDeath;

        public void apply(PowerAmount powerAmount) {
            characterData.applyDamage(powerAmount);
            if (characterData.CurrentHp <= 0) OnDeath?.Invoke(this);
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterInventoryFacade.canPlace(new PlaceItemQuery(equipItemQuery.itemDefinition,
                equipItemQuery.origin));
        }

        public ICharacterEquippedItem equipItemOrThrow(EquipItemCommand equipItemCommand) {
            if (!canPlaceItem(new EquipItemQuery(equipItemCommand.itemDefinition, equipItemCommand.origin))) {
                throw new ArgumentException("Cannot equip item");
            }

            return characterInventoryFacade.place(
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin));
        }

        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        public static CharacterAggregate createFrom(CharacterCreateCommand characterCreateCommand,
            ICharacterInventory characterInventoryFacade) {
            return new CharacterAggregate(CharacterData.from(characterCreateCommand), characterInventoryFacade,
                characterCreateCommand.team);
        }

        public Team getTeam() {
            return team;
        }

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        private void handleCharacterDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }
    }
}