using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContextRuntime {
    public record EnqueueResolvedDamagePacketCombatCommand(
        Id<CharacterId> targetCharacterId,
        Id<CharacterId> sourceCharacterId,
        ResolvedDamage resolvedDamage) : CombatCommand;
}