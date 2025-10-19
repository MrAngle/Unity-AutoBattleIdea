using System;

namespace Character {
    public class CharacterData {
        private int _currentHp;
        internal int MaxHp;
        internal string Name;

        internal CharacterData(string name, int maxHP) {
            Name = name;
            MaxHp = maxHP;
            CurrentHp = maxHP;
        }

        internal int CurrentHp {
            get => _currentHp;
            private set {
                if (_currentHp == value) {
                    return;
                }
                int _hpBeforeChange = _currentHp;
                _currentHp = value;
                OnHpChanged?.Invoke(this, _currentHp, _hpBeforeChange);
            }
        }

        internal event Action<CharacterData, int, int> OnHpChanged;

        internal void TakeDamage(int dmg) {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        internal void Heal(int amount) {
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}