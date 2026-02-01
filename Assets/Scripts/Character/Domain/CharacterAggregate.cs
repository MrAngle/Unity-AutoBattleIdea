using System;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain {
    public class CharacterAggregate : ICharacter /*, IPlacedItemOwner*/ {
        private readonly CharacterData characterData;
        private readonly ICharacterInventoryFacade characterInventoryFacade;
        private readonly Team team;

        private CharacterAggregate(CharacterData data, ICharacterInventoryFacade characterInventoryFacade, Team team) {
            this.characterData = NullGuard.NotNullOrThrow(data);
            this.characterInventoryFacade = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.team = NullGuard.enumDefinedOrThrow(team);

            // Subskrybuj event z danych, by przekazywać go dalej
            characterData.OnHpChanged += handleCharacterDataHpChanged;
        }

        public static CharacterAggregate createFrom(CharacterCreateCommand characterCreateCommand,
            ICharacterInventoryFacade characterInventoryFacade) {
            return new CharacterAggregate(CharacterData.from(characterCreateCommand), characterInventoryFacade,
                characterCreateCommand.team);
        }

        public string getName() {
            return characterData.getName();
        }

        public ICharacterInventoryFacade getInventoryAggregate() {
            return characterInventoryFacade;
        }

        public long getMaxHp() {
            return characterData.getMaxHp();
        }

        public long getCurrentHp() {
            return characterData.CurrentHp;
        }

        public long MaxHp => characterData.getMaxHp();

        public long CurrentHp => characterData.CurrentHp;

        public Team getTeam() {
            return team;
        }

        public event Action<ICharacter, long, long> OnHpChanged;
        public event Action<ICharacter> OnDeath;

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        private void handleCharacterDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }

        // Metody przepuszczające do _data


        public void apply(DamageAmount damageAmount) {
            characterData.applyDamage(damageAmount);
            if (characterData.CurrentHp <= 0) {
                OnDeath?.Invoke(this);
            }
        }

        public bool canPlaceItem(IPlaceableItem item, Vector2Int origin) {
            return characterInventoryFacade.CanPlace(item, origin);
        }

        public bool equipItemOrThrow(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem) {
            if (!canPlaceItem(item, origin)) {
                throw new ArgumentException("Cannot equip item");
            }

            placedItem = characterInventoryFacade.Place(item, origin);
            return true;
        }


        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }
    }
}