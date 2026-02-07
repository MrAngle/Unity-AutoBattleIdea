using System.Runtime.CompilerServices;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Contract;
using MageFactory.Shared.Model;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterFactoryService : ICharacterFactory {
        // private readonly IEntryPointFactory _entryPointFactory; // for now
        private readonly IInventoryFactory inventoryFactory;

        [Inject]
        internal CharacterFactoryService(
            SignalBus signalBus,
            IInventoryFactory inventoryFactory
            // , IEntryPointFactory entryPointFactory
        ) {
            this.inventoryFactory = inventoryFactory;
            // _entryPointFactory = entryPointFactory;
        }

        // @Override
        public ICharacter create(CharacterCreateCommand command) {
            ICharacterInventory characterInventory = inventoryFactory.createCharacterInventory();
            CharacterAggregate character = CharacterAggregate.createFrom(command, characterInventory);

            if (character.getTeam() == Team.TeamA) {
                foreach (EquipItemCommand itemToEquip in command.itemsToEquip) {
                    // IPlaceableItem entryPointArchetype =
                    //     _entryPointFactory.createArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
                    character.equipItemOrThrow(itemToEquip);
                }

                //for now
                // IPlaceableItem entryPointArchetype =
                //     _entryPointFactory.createArchetypeEntryPoint(FlowKind.Damage, ShapeCatalog.Square1x1);
                // character.equipItemOrThrow(entryPointArchetype, new Vector2Int(0, 0), out _);
            }

            return character;
        }
    }
}