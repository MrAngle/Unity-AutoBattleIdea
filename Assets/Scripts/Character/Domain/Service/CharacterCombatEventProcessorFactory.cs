using System.Runtime.CompilerServices;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterCombatEventProcessorFactory {
        private readonly IncomingAttackDamageCombatEventProcessor incomingAttackDamageCombatEventProcessor;

        [Inject]
        internal CharacterCombatEventProcessorFactory(CombatRuntimeSettings combatRuntimeSettings) {
            incomingAttackDamageCombatEventProcessor =
                NullGuard.NotNullOrThrow(new IncomingAttackDamageCombatEventProcessor(combatRuntimeSettings));
        }

        internal CharacterCombatEventProcessor create() {
            return new CharacterCombatEventProcessor(incomingAttackDamageCombatEventProcessor);
        }
    }
}