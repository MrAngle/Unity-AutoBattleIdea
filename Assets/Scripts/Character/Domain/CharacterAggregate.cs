using System;
using Contracts.Character;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using Shared.Utility;
using UnityEngine;

namespace Character.Domain {
    public enum Team {
        TeamA,
        TeamB
    }

    public class CharacterAggregate : ICharacter /*, IPlacedItemOwner*/ {
        private readonly CharacterData _data;
        private readonly ICharacterInventoryFacade _characterInventoryFacade;

        public CharacterAggregate(CharacterData data, ICharacterInventoryFacade characterInventoryFacade, Team team) {
            _data = NullGuard.NotNullOrThrow(data);
            _characterInventoryFacade = NullGuard.NotNullOrThrow(characterInventoryFacade);
            Team = team;

            // Subskrybuj event z danych, by przekazywać go dalej
            _data.OnHpChanged += HandleDataHpChanged;
        }

        public string getName() {
            return _data.Name;
        }

        public ICharacterInventoryFacade getInventoryAggregate() {
            return _characterInventoryFacade;
        }

        public long getMaxHp() {
            return _data.MaxHp;
        }

        public long getCurrentHp() {
            return _data.CurrentHp;
        }

        public long MaxHp => _data.MaxHp;

        public long CurrentHp => _data.CurrentHp;

        public Team Team { get; }

        public event Action<ICharacter, long, long> OnHpChanged;
        public event Action<ICharacter> OnDeath;

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            _data.OnHpChanged -= HandleDataHpChanged;
        }

        private void HandleDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }

        // Metody przepuszczające do _data


        public void apply(DamageAmount damageAmount) {
            _data.Apply(damageAmount);
            if (_data.CurrentHp <= 0) {
                OnDeath?.Invoke(this);
            }
        }

        public bool canPlaceItem(IPlaceableItem item, Vector2Int origin) {
            return _characterInventoryFacade.CanPlace(item, origin);
        }

        public bool equipItemOrThrow(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem) {
            if (!canPlaceItem(item, origin)) {
                throw new ArgumentException("Cannot equip item");
            }

            placedItem = _characterInventoryFacade.Place(item, origin);
            return true;
        }


        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void cleanup() {
            _data.OnHpChanged -= HandleDataHpChanged;
        }
    }
}