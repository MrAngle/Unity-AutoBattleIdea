namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatCapabilitiesFactory {
        ICharacterCombatCapabilities createCombatContextFactory(ICombatCharacter character);
    }
}