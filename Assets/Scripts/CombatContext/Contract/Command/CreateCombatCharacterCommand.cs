using System.Collections.Generic;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract.Command {
    public record CreateCombatCharacterCommand(
        string name,
        int maxHp,
        Team team,
        IReadOnlyList<EquipItemCommand> itemsToEquip);
}