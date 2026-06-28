using MageFactory.CombatEvents;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContextRuntime {
    public interface ICombatCapabilities {
        ICombatCommandBus command();
        ICombatQueries query();
    }

    public interface ICombatCommandBus {
        void dispatch(CombatCommand combatCommand);
    }

    public interface ICombatQueries {
        // remember to return only simple types like int, float, bool, string, enum etc.
        int getActiveFlowCount();
        int getActiveFlowCountForCharacter(Id<CharacterId> characterId);
        int getCreatedFlowCount();
        int getCreatedFlowCountForCharacter(Id<CharacterId> characterId);
        int getCombatEventCount(CombatEventType combatEventType);
        int getActiveDamagePacketCount();
    }
}