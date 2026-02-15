using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
// using MageFactory.CombatContext.Api.Dto;

namespace MageFactory.CombatContext.Domain {
    internal class CombatContext : ICombatContext {
        private readonly Dictionary<Id<CharacterId>, ICombatCharacter> characters = new();

        private readonly ICharacterFactory characterFactory;

        private CombatContext(ICharacterFactory characterFactory) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            NullGuard.NotNullOrThrow(characters);

            // this.characters = NullGuard.NotNullOrThrow(characters);
        }

        internal static CombatContext create(ICharacterFactory paramCharacterFactory,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(paramCharacterFactory);

            foreach (CreateCombatCharacterCommand createCombatCharacterCommand in charactersToCreate) {
                combatContext.registerCharacter(createCombatCharacterCommand);
            }

            return combatContext;
        }

        private void registerCharacter(CreateCombatCharacterCommand createCombatCharacterCommand) {
            ICombatCharacter combatCharacter = characterFactory.create(createCombatCharacterCommand);

            characters.Add(combatCharacter.getId(), combatCharacter);
        }

        // public CharacterSummaryView getSummary(long id) {
        //     var character = characters[id];
        //     return new CharacterSummaryView(
        //         character.Id,
        //         character.Name,
        //         character.CurrentHp,
        //         character.MaxHp
        //     );
        // }
        public ICombatCharacter getRandomCharacter() {
            return characters.Values.FirstOrDefault();
        }

        public IReadOnlyCollection<ICombatCharacter> getAllCharacters() {
            return characters.Values;
        }
    }
}