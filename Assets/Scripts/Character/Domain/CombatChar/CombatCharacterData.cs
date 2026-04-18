using MageFactory.Character.Domain.Service;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterData : IReadOnlyCombatCharacterData {
        private readonly IReadOnlyCharacterData characterData;
        private readonly Team team;

        public CombatCharacterData(IReadOnlyCharacterData characterData, Team team) {
            this.characterData = NullGuard.NotNullOrThrow(characterData);
            this.team = NullGuard.enumDefinedOrThrow(team);
        }

        public Team getTeam() {
            return team;
        }

        public Id<CharacterId> getCharacterId() {
            return characterData.getCharacterId();
        }

        public string getCharacterName() {
            return characterData.getCharacterName();
        }

        public long getMaxHp() {
            return characterData.getMaxHp();
        }

        public long getCurrentHp() {
            return characterData.getCurrentHp();
        }
    }
}