using MageFactory.Shared.Id;

namespace MageFactory.CombatContextRuntime {
    public record CreateFlowCombatCommand(
        Id<CharacterId> characterId,
        Id<ItemId> itemId) : CombatCommand;
}