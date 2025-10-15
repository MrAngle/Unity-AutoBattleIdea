using System;

namespace Character
{
    public enum Team
    {
        TeamA,
        TeamB
    }

    public class CharacterAggregate
    {
        private CharacterData _data;
        private Team _team;

        public event Action<CharacterAggregate, int> OnHpChanged;
        public event Action<CharacterAggregate> OnDeath;

        public CharacterAggregate(CharacterData data, Team team)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _team = team;

            // Subskrybuj event z danych, by przekazywać go dalej
            _data.OnHpChanged += HandleDataHpChanged;
        }
        
        public string Name => _data.Name;

        public int MaxHp => _data.MaxHp;

        public int CurrentHp => _data.CurrentHp;

        public Team Team => _team;

        ~CharacterAggregate()
        {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            _data.OnHpChanged -= HandleDataHpChanged;
        }

        private void HandleDataHpChanged(CharacterData data, int newHp)
        {
            OnHpChanged?.Invoke(this, newHp);
        }



        // Metody przepuszczające do _data
        public void TakeDamage(int dmg)
        {
            _data.TakeDamage(dmg);
            if (_data.CurrentHp <= 0)
            {
                OnDeath?.Invoke(this);
            }
        }

        public void Heal(int amount)
        {
            _data.Heal(amount);
        }

        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void Cleanup()
        {
            _data.OnHpChanged -= HandleDataHpChanged;
        }

    }
}