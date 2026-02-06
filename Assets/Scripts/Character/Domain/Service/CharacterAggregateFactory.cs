using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace MageFactory.Character.Domain.Service {
    public class CharacterAggregateFactory : ICharacterAggregateFactory {
        private readonly IEntryPointFactory _entryPointFactory; // for now
        private readonly IInventoryAggregateFactory _inventoryAggregateFactory;

        [Inject]
        public CharacterAggregateFactory(
            SignalBus signalBus,
            IInventoryAggregateFactory inventoryAggregateFactory,
            IEntryPointFactory entryPointFactory) {
            _inventoryAggregateFactory = inventoryAggregateFactory;
            _entryPointFactory = entryPointFactory;
        }

        // @Override
        public ICharacter create(CharacterCreateCommand command) {
            ICharacterInventoryFacade characterInventory = _inventoryAggregateFactory.CreateCharacterInventory();
            CharacterAggregate character = CharacterAggregate.createFrom(command, characterInventory);

            if (character.getTeam() == Team.TeamA) {
                //for now
                IPlaceableItem entryPointArchetype =
                    _entryPointFactory.CreateArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
                character.equipItemOrThrow(entryPointArchetype, new Vector2Int(0, 0), out _);
            }

            return character;
        }
    }
}