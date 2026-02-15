using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Domain.Service {
    internal sealed class CombatContextFactoryService : ICombatContextFactory {
        private readonly ICharacterFactory characterFactory;

        [Inject]
        public CombatContextFactoryService(ICharacterFactory injectedCharacterFactory) {
            characterFactory = NullGuard.NotNullOrThrow(injectedCharacterFactory);
        }

        public ICombatContext create(IReadOnlyList<CreateCombatCharacterCommand> createCombatCharacterCommands) {
            return CombatContext.create(characterFactory, createCombatCharacterCommands);
        }
    }
}