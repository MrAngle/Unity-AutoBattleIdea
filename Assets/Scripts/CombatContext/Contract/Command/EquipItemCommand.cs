using MageFactory.ActionEffect;
using UnityEngine;

namespace MageFactory.CombatContext.Contract.Command {
    public record EquipItemCommand(IItemDefinition itemDefinition, Vector2Int origin);
}