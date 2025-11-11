using Character;
using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.EntryPoints {
    public interface IEntryPointOwner : IPlacedItemOwner {
        
    }
    
    public interface IPlacedEntryPoint : IPlacedItem {
        void StartBattle();
    }
}