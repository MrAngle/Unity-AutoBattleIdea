using System.Runtime.CompilerServices;
using MageFactory.Character.Domain.CharacterCapability;
using MageFactory.Character.Domain.CombatChar;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterCombatCapabilitiesFactory {
        // [Inject] // tutaj pojawia sie ewentualne potrzebne serwisy
        public CombatCharacterFacade createCombatContextFactory(CombatCharacter combatCharacter) {
            return new CombatCharacterFacade(combatCharacter);
        }
    }
}