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

        internal event Action<CharacterData, long, long> onHpChanged;

        private CharacterData(string name, int maxHp) {
            this.characterId = new Id<CharacterId>(IdGenerator.Next());
            this.name = name;
            this.maxHp = maxHp;
            this.currentHp = maxHp;
        }

        internal static CharacterData from(CreateCombatCharacterCommand command) {
            return new CharacterData(command.name, command.maxHp);
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

        internal DamageTaken takeDamage(ResolvedDamage resolvedDamage) {
            if (resolvedDamage == null) {
                throw new ArgumentNullException(nameof(resolvedDamage));
            }

            return takeDamage(resolvedDamage.getPower());
        }

        private DamageTaken takeDamage(long damage) {
            if (damage < 0) {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }

            long hpBeforeChange = currentHp;
            long hpAfterChange = Math.Max(0, currentHp - damage);
            long actualDamageReceived = hpBeforeChange - hpAfterChange;

            if (hpBeforeChange != hpAfterChange) {
                currentHp = hpAfterChange;
            }

            onHpChanged?.Invoke(this, currentHp, hpBeforeChange);
            return new DamageTaken(actualDamageReceived);
        }
    }
}