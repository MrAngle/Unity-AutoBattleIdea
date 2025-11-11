using Combat.Flow.Domain.Aggregate;
using Inventory;
using Inventory.EntryPoints;
using Inventory.Items.Config;
using UnityEngine;
using Zenject;

namespace Character {
    
    public interface ICharacterAggregateFactory {
        ICharacterAggregateFacade Create(CharacterData characterData, Team team);
    }


    public class CharacterAggregateFactory : ICharacterAggregateFactory {
        private readonly IInventoryAggregateFactory _inventoryAggregateFactory;
        private readonly IEntryPointFactory _entryPointFactory; // for now

        [Inject]
        public CharacterAggregateFactory(
            SignalBus signalBus,
            IInventoryAggregateFactory inventoryAggregateFactory,
            IEntryPointFactory entryPointFactory) {
            _inventoryAggregateFactory = inventoryAggregateFactory;
            _entryPointFactory = entryPointFactory;
        }

        public ICharacterAggregateFacade Create(CharacterData characterData, Team team) {
            ICharacterInventoryFacade characterInventory = _inventoryAggregateFactory.CreateCharacterInventory();

            CharacterAggregate characterAggregate = new CharacterAggregate(characterData, characterInventory, team);
            
            EntryPointArchetype entryPointArchetype =
                _entryPointFactory.CreateArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
            characterAggregate.TryEquipItem(entryPointArchetype, new Vector2Int(0, 0), out _);

            return characterAggregate;
        }
    }
}