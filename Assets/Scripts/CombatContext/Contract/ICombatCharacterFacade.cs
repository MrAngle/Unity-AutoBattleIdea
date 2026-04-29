namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterFacade {
        ICharacterCombatCommandBus command();
        ICharacterCombatQueries query();
    }
}