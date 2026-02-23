using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain {
    internal class CombatContext : ICombatContext {
        private readonly Dictionary<Id<CharacterId>, ICombatCharacter> characters = new();

        private readonly ICharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;

        private CombatContext(ICharacterFactory characterFactory,
                              ICombatContextEventPublisher combatContextEventPublisher) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);
            NullGuard.NotNullOrThrow(characters);
        }

        internal static CombatContext create(ICharacterFactory paramCharacterFactory,
                                             ICombatContextEventPublisher combatContextEventPublisher,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(paramCharacterFactory, combatContextEventPublisher);

            foreach (CreateCombatCharacterCommand createCombatCharacterCommand in charactersToCreate) {
                combatContext.registerCharacter(createCombatCharacterCommand);
            }

            combatContextEventPublisher.publish(new CombatContextCreatedDtoEvent(combatContext));
            return combatContext;
        }

        private void registerCharacter(CreateCombatCharacterCommand createCombatCharacterCommand) {
            ICombatCharacter combatCharacter = characterFactory.create(createCombatCharacterCommand);

            Id<CharacterId> characterId = combatCharacter.getId();
            if (characters.ContainsKey(characterId)) {
                throw new InvalidOperationException(
                    $"Character with id '{characterId}' is already registered in CombatContext.");
            }

            characters.Add(combatCharacter.getId(), combatCharacter);
            combatContextEventPublisher.publish(new CombatCharacterCreatedDtoEvent());
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

        public ICombatCharacter getCombatCharacterById(Id<CharacterId> id) {
            return characters[id];
        }
    }
}