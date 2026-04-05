using MageFactory.ActionEffect;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public record PlaceItemCommand(
        IItemDefinition itemDefinition,
        Vector2Int origin);
}