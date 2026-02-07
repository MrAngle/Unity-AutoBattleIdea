using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Item.Api;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using Zenject;

namespace MageFactory.Character.Domain.Service {
    public class CharacterAggregateFactory : ICharacterAggregateFactory {
        private readonly IEntryPointFactory _entryPointFactory; // for now
        private readonly IInventoryFactory inventoryFactory;

        [Inject]
        public CharacterAggregateFactory(
            SignalBus signalBus,
            IInventoryFactory inventoryFactory,
            IEntryPointFactory entryPointFactory) {
            this.inventoryFactory = inventoryFactory;
            _entryPointFactory = entryPointFactory;
        }

        // @Override
        public ICharacter create(CharacterCreateCommand command) {
            ICharacterInventoryFacade characterInventory = inventoryFactory.CreateCharacterInventory();
            CharacterAggregate character = CharacterAggregate.createFrom(command, characterInventory);

            if (character.getTeam() == Team.TeamA) {
                //for now
                IPlaceableItem entryPointArchetype =
                    _entryPointFactory.createArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
                character.equipItemOrThrow(entryPointArchetype, new Vector2Int(0, 0), out _);
            }

            return character;
        }
    }
}