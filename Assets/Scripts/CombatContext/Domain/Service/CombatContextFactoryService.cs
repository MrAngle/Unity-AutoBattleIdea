using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Domain.Service {
    internal sealed class CombatContextFactoryService : ICombatContextFactory {
        private readonly ICombatCharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;
        private readonly CombatRuntimeSettings combatRuntimeSettings;
        private readonly DamagePacketProcessingSettings damagePacketProcessingSettings;

        [Inject]
        public CombatContextFactoryService(ICombatCharacterFactory injectedCharacterFactory,
                                           ICombatContextEventPublisher injectedContextEventPublisher,
                                           CombatRuntimeSettings injectedCombatRuntimeSettings,
                                           DamagePacketProcessingSettings injectedDamagePacketProcessingSettings) {
            characterFactory = NullGuard.NotNullOrThrow(injectedCharacterFactory);
            combatContextEventPublisher = NullGuard.NotNullOrThrow(injectedContextEventPublisher);
            combatRuntimeSettings = NullGuard.NotNullOrThrow(injectedCombatRuntimeSettings);
            damagePacketProcessingSettings = NullGuard.NotNullOrThrow(injectedDamagePacketProcessingSettings);
        }

        public ICombatContext create(IReadOnlyList<CreateCombatCharacterCommand> createCombatCharacterCommands) {
            return CombatContext.create(
                characterFactory,
                combatContextEventPublisher,
                combatRuntimeSettings,
                damagePacketProcessingSettings,
                createCombatCharacterCommands);
        }
    }
}