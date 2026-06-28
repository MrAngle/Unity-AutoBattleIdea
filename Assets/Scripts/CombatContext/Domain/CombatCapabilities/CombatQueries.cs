using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain.CombatCapabilities {
    internal class CombatQueries : ICombatQueries {
        private readonly IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters;
        private readonly IReadOnlyDictionary<CombatEventType, int> combatEventCountsByType;
        private readonly IReadOnlyCollection<ActiveDamagePacket> activeDamagePackets;

        internal CombatQueries(IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters,
                               IReadOnlyDictionary<CombatEventType, int> combatEventCountsByType,
                               IReadOnlyCollection<ActiveDamagePacket> activeDamagePackets) {
            this.characters = NullGuard.NotNullOrThrow(characters);
            this.combatEventCountsByType = NullGuard.NotNullOrThrow(combatEventCountsByType);
            this.activeDamagePackets = NullGuard.NotNullOrThrow(activeDamagePackets);
        }

        public int getActiveFlowCount() {
            var activeFlowCount = 0;

            foreach (ICombatCharacterFacade character in characters.Values) {
                activeFlowCount += character.query().getActiveFlowCount();
            }

            return activeFlowCount;
        }

        public int getActiveFlowCountForCharacter(Id<CharacterId> characterId) {
            NullGuard.ValidIdOrThrow(characterId);

            return characters.TryGetValue(characterId, out ICombatCharacterFacade character)
                ? character.query().getActiveFlowCount()
                : 0;
        }

        public int getCreatedFlowCount() {
            var createdFlowCount = 0;

            foreach (ICombatCharacterFacade character in characters.Values) {
                createdFlowCount += character.query().getCreatedFlowsInCurrentBattleCount();
            }

            return createdFlowCount;
        }

        public int getCreatedFlowCountForCharacter(Id<CharacterId> characterId) {
            NullGuard.ValidIdOrThrow(characterId);

            return characters.TryGetValue(characterId, out ICombatCharacterFacade character)
                ? character.query().getCreatedFlowsInCurrentBattleCount()
                : 0;
        }

        public int getCombatEventCount(CombatEventType combatEventType) {
            NullGuard.enumDefinedOrThrow(combatEventType);

            return combatEventCountsByType.TryGetValue(combatEventType, out int count)
                ? count
                : 0;
        }

        public int getActiveDamagePacketCount() {
            return activeDamagePackets.Count;
        }
    }
}