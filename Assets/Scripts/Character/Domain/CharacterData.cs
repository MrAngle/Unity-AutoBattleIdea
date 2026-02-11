using System;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain {
    internal class CharacterData {
        private long currentHp;
        private int maxHp;
        private string name;

        private CharacterData(string name, int maxHp) {
            this.name = name;
            this.maxHp = maxHp;
            this.currentHp = maxHp;
        }

        internal static CharacterData from(CreateCombatCharacterCommand command) {
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

        internal void applyDamage(PowerAmount damageAmount) {
            switch (damageAmount) {
                case DamageToDeal deal:
                    takeDamage(deal.getPower());
                    break;
                case DamageToReceive receive:
                    heal(receive.getPower());
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