using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryCombatTickableItem {
        void tick(CombatTicks combatTicks, Id<CharacterId> characterId, ICombatCapabilities combatCapabilities);
    }
}