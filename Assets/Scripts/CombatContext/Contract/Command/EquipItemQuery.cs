using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.CombatContext.Contract.Command {
    public record EquipItemQuery(IItemDefinition itemDefinition, Vector2Int origin);
}