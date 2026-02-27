using System.Runtime.CompilerServices;
using MageFactory.Character.Domain.CharacterCapability;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterCombatCapabilitiesFactory {
        // [Inject] // tutaj pojawia sie ewentualne potrzebne serwisy
        public CharacterCombatCapabilities createCombatContextFactory(CharacterAggregate paramCharacter) {
            return new CharacterCombatCapabilities(paramCharacter);
        }
    }
}