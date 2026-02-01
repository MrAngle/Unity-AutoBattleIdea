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
            return new CharacterData(command.name, command.maxHp);
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
                var hpBeforeChange = currentHp;
                currentHp = value;
                OnHpChanged?.Invoke(this, currentHp, hpBeforeChange);
            }
        }

        internal event Action<CharacterData, long, long> OnHpChanged;

        internal void applyDamage(DamageAmount damageAmount) {
            switch (damageAmount) {
                case DamageToDeal deal:
                    takeDamage(deal.GetPower());
                    break;
                case DamageToReceive receive:
                    heal(receive.GetPower());
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported damage type: {damageAmount.GetType().Name}");
            }
        }

        private void takeDamage(long dmg) {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        private void heal(long amount) {
            CurrentHp += amount;
            if (CurrentHp > maxHp) CurrentHp = maxHp;
        }
    }
}