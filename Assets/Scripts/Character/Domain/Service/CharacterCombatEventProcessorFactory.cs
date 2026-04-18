using System.Runtime.CompilerServices;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterCombatEventProcessorFactory {
        private readonly DamageIncomingCombatEventProcessor damageIncomingCombatEventProcessor;

        [Inject]
        internal CharacterCombatEventProcessorFactory(
        ) {
            this.damageIncomingCombatEventProcessor =
                NullGuard.NotNullOrThrow(new DamageIncomingCombatEventProcessor());
        }

        internal CharacterCombatEventProcessor create() {
            return new CharacterCombatEventProcessor(damageIncomingCombatEventProcessor);
        }
    }
}