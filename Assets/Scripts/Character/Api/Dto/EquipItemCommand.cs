using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.Character.Api.Dto {
    public record EquipItemCommand(IItemDefinition itemDefinition, Vector2Int origin);
}