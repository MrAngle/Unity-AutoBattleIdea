using System;
using Combat.Flow.Domain.Shared;
using Inventory;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace Character {
    public enum Team {
        TeamA,
        TeamB
    }

    public interface IPlacedItemOwner {
        
    }
    
    public interface ICharacterAggregateFacade {
        public event Action<CharacterAggregate, long, long> OnHpChanged;
        public event Action<CharacterAggregate> OnDeath;

        bool TryEquipItem(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem);
        bool CanPlaceItem(IPlaceableItem item, Vector2Int origin);
        
        ICharacterInventoryFacade GetInventoryAggregate();

        public long GetMaxHp();

        public long GetCurrentHp();
        
        void Apply(DamageAmount damageAmount);
        
        string GetName();

        void Cleanup();
    }
    

    public class CharacterAggregate : ICharacterAggregateFacade, IPlacedItemOwner {
        private readonly CharacterData _data;
        private readonly ICharacterInventoryFacade _characterInventoryFacade;

        public CharacterAggregate(CharacterData data, ICharacterInventoryFacade characterInventoryFacade, Team team) {
            _data = NullGuard.NotNullOrThrow(data);
            _characterInventoryFacade = NullGuard.NotNullOrThrow(characterInventoryFacade);
            Team = team;

            // Subskrybuj event z danych, by przekazywać go dalej
            _data.OnHpChanged += HandleDataHpChanged;
        }

        public string GetName() {
            return _data.Name;
        }

        public ICharacterInventoryFacade GetInventoryAggregate() {
            return _characterInventoryFacade;
        }

        public long GetMaxHp() {
            return _data.MaxHp;
        }

        public long GetCurrentHp() {
            return _data.CurrentHp;
        }

        public long MaxHp => _data.MaxHp;

        public long CurrentHp => _data.CurrentHp;

        public Team Team { get; }

        public event Action<CharacterAggregate, long, long> OnHpChanged;
        public event Action<CharacterAggregate> OnDeath;

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            _data.OnHpChanged -= HandleDataHpChanged;
        }

        private void HandleDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }

        // Metody przepuszczające do _data


        public void Apply(DamageAmount damageAmount) {
            _data.Apply(damageAmount);
            if (_data.CurrentHp <= 0) {
                OnDeath?.Invoke(this);
            }
        }

        public bool CanPlaceItem(IPlaceableItem item, Vector2Int origin) {
            return _characterInventoryFacade.CanPlace(item, origin);
        }
        
        public bool TryEquipItem(IPlaceableItem item, Vector2Int origin, out IPlacedItem placedItem) {
            if (!CanPlaceItem(item, origin)) {
                throw new ArgumentException("Cannot equip item");
            }

            placedItem = _characterInventoryFacade.Place(this, item, origin);
            return true;
        }
        

        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void Cleanup() {
            _data.OnHpChanged -= HandleDataHpChanged;
        }
    }
}