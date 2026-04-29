using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Id;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryCombatTickableItem {
        void tick(Id<CharacterId> characterId, ICombatCapabilities combatCapabilities);
    }
}