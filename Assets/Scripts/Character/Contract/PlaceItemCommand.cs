using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public record PlaceItemCommand(
        IItemDefinition itemDefinition,
        Vector2Int origin,
        ICharacterCombatCapabilities characterCombatCapabilities);
}