using System.Collections.Generic;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Api.Dto {
    public record CharacterCreateCommand(
        string name,
        int maxHp,
        Team team,
        IReadOnlyList<EquipItemCommand> itemsToEquip
    );
}