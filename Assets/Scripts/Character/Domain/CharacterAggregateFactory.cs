using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using Inventory;
using Inventory.EntryPoints;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace Character.Domain {
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
        //
        // public ICharacter Create(CharacterData characterData, Team team) {
        //     var characterInventory = _inventoryAggregateFactory.CreateCharacterInventory();
        //
        //     var characterAggregate = new CharacterAggregate(characterData, characterInventory, team);
        //
        //     if (team == Team.TeamA) {
        //         //for now
        //         var entryPointArchetype =
        //             _entryPointFactory.CreateArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
        //         characterAggregate.equipItemOrThrow(entryPointArchetype, new Vector2Int(0, 0), out _);
        //     }
        //
        //     return characterAggregate;
        // }

        // @Override
        public ICharacter Create(CharacterCreateCommand command) {
            ICharacterInventoryFacade characterInventory = _inventoryAggregateFactory.CreateCharacterInventory();
            CharacterAggregate character = CharacterAggregate.createFrom(command, characterInventory);

            if (character.getTeam() == Team.TeamA) {
                //for now
                var entryPointArchetype =
                    _entryPointFactory.CreateArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
                character.equipItemOrThrow(entryPointArchetype, new Vector2Int(0, 0), out _);
            }

            return character;
        }
    }
}