namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacterFacade {
        ICombatCommandBus command();
        ICombatQueries query();
    }
}