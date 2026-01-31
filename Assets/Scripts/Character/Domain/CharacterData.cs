using System;
using Contracts.Flow;

// using Combat.Flow.Domain.Shared;

namespace Character.Domain {
    public class CharacterData {
        private long _currentHp;
        internal int MaxHp;
        internal string Name;

        public CharacterData(string name, int maxHP) {
            // TO internal
            Name = name;
            MaxHp = maxHP;
            CurrentHp = maxHP;
        }

        internal long CurrentHp {
            get => _currentHp;
            private set {
                if (_currentHp == value) return;
                var _hpBeforeChange = _currentHp;
                _currentHp = value;
                OnHpChanged?.Invoke(this, _currentHp, _hpBeforeChange);
            }
        }

        internal event Action<CharacterData, long, long> OnHpChanged;

        internal void Apply(DamageAmount damageAmount) {
            switch (damageAmount) {
                case DamageToDeal deal:
                    TakeDamage(deal.GetPower());
                    break;
                case DamageToReceive receive:
                    Heal(receive.GetPower());
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported damage type: {damageAmount.GetType().Name}");
            }
        }

        private void TakeDamage(long dmg) {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        private void Heal(long amount) {
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}