using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Domain.Service {
    internal sealed class CombatContextFactoryService : ICombatContextFactory {
        private readonly ICombatCharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;

        [Inject]
        public CombatContextFactoryService(ICombatCharacterFactory injectedCharacterFactory,
                                           ICombatContextEventPublisher injectedContextEventPublisher) {
            characterFactory = NullGuard.NotNullOrThrow(injectedCharacterFactory);
            combatContextEventPublisher = NullGuard.NotNullOrThrow(injectedContextEventPublisher);
        }

        public ICombatContext create(IReadOnlyList<CreateCombatCharacterCommand> createCombatCharacterCommands) {
            return CombatContext.create(characterFactory, combatContextEventPublisher, createCombatCharacterCommands);
        }
    }
}