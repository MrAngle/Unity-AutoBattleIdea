using System;
using MageFactory.Character.Domain.Service;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterData : IReadOnlyCharacterData {
        private readonly Id<CharacterId> characterId;
        private long currentHp;
        private int maxHp;
        private string name;

        internal event Action<CharacterData, long, long> OnHpChanged;

        private CharacterData(string name, int maxHp) {
            this.characterId = new Id<CharacterId>(IdGenerator.Next());
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

        public Id<CharacterId> getCharacterId() {
            return characterId;
        }

        public string getCharacterName() {
            return name;
        }

        public long getMaxHp() {
            return maxHp;
        }

        public long getCurrentHp() {
            return currentHp;
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