using System;
using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain.CombatCapabilities {
    namespace MageFactory.CombatContext.Domain {
        internal class CombatQueries : ICombatQueries {
            private readonly IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters;

            internal CombatQueries(IReadOnlyDictionary<Id<CharacterId>, ICombatCharacterFacade> characters) {
                this.characters = NullGuard.NotNullOrThrow(characters);
            }

            public int getActiveFlowCount() {
                throw new NotImplementedException();
            }

            public int getActiveFlowCountForCharacter(Id<CharacterId> characterId) {
                throw new NotImplementedException();
            }

            public int getCreatedFlowCount() {
                throw new NotImplementedException();
            }

            public int getCreatedFlowCountForCharacter(Id<CharacterId> characterId) {
                throw new NotImplementedException();
            }
        }
    }
}