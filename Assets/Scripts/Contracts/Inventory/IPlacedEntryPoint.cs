using Contracts.Items;

namespace Contracts.Inventory {
    public interface IPlacedEntryPoint : IPlacedItem {
        void StartBattle();
    }
}