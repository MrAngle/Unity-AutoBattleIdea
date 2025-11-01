using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.EntryPoints {
    public interface IPlacedEntryPoint : IPlacedItem {
        void StartBattle();
    }
}