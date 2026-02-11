namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatCapabilities {
        ICombatCommandBus command();
        ICombatQueries query();
    }
}