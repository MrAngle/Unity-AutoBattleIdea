using System;

namespace Character {
    public class CharacterData {
        private long _currentHp;
        internal int MaxHp;
        internal string Name;

        internal CharacterData(string name, int maxHP) {
            Name = name;
            MaxHp = maxHP;
            CurrentHp = maxHP;
        }

        internal long CurrentHp {
            get => _currentHp;
            private set {
                if (_currentHp == value) {
                    return;
                }
                long _hpBeforeChange = _currentHp;
                _currentHp = value;
                OnHpChanged?.Invoke(this, _currentHp, _hpBeforeChange);
            }
        }

        internal event Action<CharacterData, long, long> OnHpChanged;

        internal void TakeDamage(long dmg) {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        internal void Heal(long amount) {
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}