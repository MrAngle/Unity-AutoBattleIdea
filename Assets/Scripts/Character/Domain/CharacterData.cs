using System;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterData {
        private long currentHp;
        private int maxHp;
        private string name;
        private Team team;

        internal event Action<CharacterData, long, long> OnHpChanged;

        private CharacterData(string name, int maxHp, Team team) {
            this.name = name;
            this.maxHp = maxHp;
            this.currentHp = maxHp;
            this.team = NullGuard.enumDefinedOrThrow(team);
        }

        internal static CharacterData from(CreateCombatCharacterCommand command) {
            return new CharacterData(command.name, command.maxHp, command.team);
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

        internal void takeDamage(DamageToReceive damageToReceive) {
            takeDamage(damageToReceive.getPower());
        }

        private void takeDamage(long dmg) {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }
    }
}