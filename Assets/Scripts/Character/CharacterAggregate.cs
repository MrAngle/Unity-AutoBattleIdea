using System;

namespace Character {
    public enum Team {
        TeamA,
        TeamB
    }

    public class CharacterAggregate {
        private readonly CharacterData _data;

        public CharacterAggregate(CharacterData data, Team team) {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            Team = team;

            // Subskrybuj event z danych, by przekazywać go dalej
            _data.OnHpChanged += HandleDataHpChanged;
        }

        public string Name => _data.Name;

        public int MaxHp => _data.MaxHp;

        public int CurrentHp => _data.CurrentHp;

        public Team Team { get; }

        public event Action<CharacterAggregate, int, int> OnHpChanged;
        public event Action<CharacterAggregate> OnDeath;

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            _data.OnHpChanged -= HandleDataHpChanged;
        }

        private void HandleDataHpChanged(CharacterData data, int newHp, int previousHpValue) {
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }


        // Metody przepuszczające do _data
        public void TakeDamage(int dmg) {
            _data.TakeDamage(dmg);
            if (_data.CurrentHp <= 0) {
                OnDeath?.Invoke(this);
            }
        }

        public void Heal(int amount) {
            _data.Heal(amount);
        }

        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void Cleanup() {
            _data.OnHpChanged -= HandleDataHpChanged;
        }
    }
}