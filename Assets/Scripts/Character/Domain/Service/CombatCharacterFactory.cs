using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CombatCharacterFactory : ICombatCharacterFactory {
        private readonly CharacterFactory characterFactory;
        private readonly CharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory;

        [Inject]
        internal CombatCharacterFactory(
            CharacterFactory characterFactory,
            CharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory
        ) {
            this.characterFactory = characterFactory;
            this.characterCombatCapabilitiesFactory = characterCombatCapabilitiesFactory;
        }

        public ICombatCharacter create(CreateCombatCharacterCommand command) {
            CharacterAggregate character = characterFactory.createCharacter(command);
            ICharacterCombatCapabilities characterCombatCapabilities =
                characterCombatCapabilitiesFactory.createCombatContextFactory(character);

            return new CombatCharacter(character, characterCombatCapabilities);
        }
    }
}