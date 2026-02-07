using MageFactory.Shared.Contract;
using UnityEngine;

namespace MageFactory.Character.Contract {
    public record PlaceItemQuery(IItemDefinition itemDefinition, Vector2Int origin);
}