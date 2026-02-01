using System;
using Contracts.Flow;
using MageFactory.Character.Api.Dto;

namespace MageFactory.Character.Domain {
    public class CharacterData {
        private long currentHp;
        private int maxHp;
        private string name;

        private CharacterData(string name, int maxHp) {
            this.name = name;
            this.maxHp = maxHp;
            this.currentHp = maxHp;
        }

        internal static CharacterData from(CharacterCreateCommand command) {
            return new CharacterData(command.Name, command.MaxHp);
        }

        internal string getName() {
            return name;
        }

        internal int getMaxHp() {
            return maxHp;
        }

        internal long CurrentHp {
            get => currentHp;
            private set {
                if (currentHp == value) return;
                var _hpBeforeChange = currentHp;
                currentHp = value;
                OnHpChanged?.Invoke(this, currentHp, _hpBeforeChange);
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
            if (CurrentHp > maxHp) CurrentHp = maxHp;
        }
    }
}