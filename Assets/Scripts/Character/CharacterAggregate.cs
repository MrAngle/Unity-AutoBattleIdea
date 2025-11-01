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
        public void TakeDamage(long dmg) {
            _data.TakeDamage(dmg);
            if (_data.CurrentHp <= 0) {
                OnDeath?.Invoke(this);
            }
        }

        public void Heal(long amount) {
            _data.Heal(amount);
        }

        // Jeśli chcesz ręcznie posprzątać (usunąć subskrypcję),
        // np. gdy obiekt jest niszczony
        public void Cleanup() {
            _data.OnHpChanged -= HandleDataHpChanged;
        }
    }
}