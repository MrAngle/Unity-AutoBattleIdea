using System.Runtime.CompilerServices;
using MageFactory.Character.Domain.CharacterCapability;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CombatCharacterFactory : ICombatCharacterFactory {
        private readonly CharacterFactory characterFactory;
        private readonly CharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory;
        private readonly IFlowFactory flowFactory;

        [Inject]
        internal CombatCharacterFactory(
            CharacterFactory characterFactory,
            CharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory,
            IFlowFactory flowFactory
        ) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.characterCombatCapabilitiesFactory = NullGuard.NotNullOrThrow(characterCombatCapabilitiesFactory);
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }

        public ICombatCharacter create(CreateCombatCharacterCommand command) {
            CharacterAggregate character = characterFactory.createCharacter(command);
            CharacterCombatCapabilities characterCombatCapabilities =
                characterCombatCapabilitiesFactory.createCombatContextFactory(character);

            return new CombatCharacter(character, characterCombatCapabilities, command.team, flowFactory);
        }
    }
}