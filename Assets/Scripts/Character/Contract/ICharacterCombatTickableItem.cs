using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;

namespace MageFactory.Character.Contract {
    public delegate void CharacterCombatTickableItemAction(
        Id<CharacterId> characterId,
        ICombatCapabilities combatCapabilities
    );
}