using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Contract {
    public delegate void CharacterCombatTickableItemAction(
        CombatTicks combatTicks,
        Id<CharacterId> characterId,
        ICombatCapabilities combatCapabilities
    );
}