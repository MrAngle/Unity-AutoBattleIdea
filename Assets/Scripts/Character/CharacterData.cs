namespace Character
{
    public class CharacterData
    {
        internal string Name;
        internal int MaxHp;
        private int _currentHp;
        internal int CurrentHp
        {
            get => _currentHp;
            private set
            {
                if (_currentHp == value) return;
                _currentHp = value;
                OnHpChanged?.Invoke(this, _currentHp);
            }
        }

        internal event System.Action<CharacterData, int> OnHpChanged;
        
        internal CharacterData(string name, int maxHP)
        {
            Name = name;
            MaxHp = maxHP;
            CurrentHp = maxHP;
        }

        internal void TakeDamage(int dmg)
        {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        internal void Heal(int amount)
        {
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}