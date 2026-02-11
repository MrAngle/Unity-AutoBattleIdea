using System.Runtime.CompilerServices;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterFactoryService : ICharacterFactory {
        private readonly ICharacterCombatCapabilitiesFactory combatCapabilitiesFactory;
        private readonly IInventoryFactory inventoryFactory;

        [Inject]
        internal CharacterFactoryService(
            IInventoryFactory inventoryFactory,
            ICharacterCombatCapabilitiesFactory combatCapabilitiesFactory
        ) {
            this.inventoryFactory = inventoryFactory;
            this.combatCapabilitiesFactory = combatCapabilitiesFactory;
        }

        public ICombatCharacter create(CreateCombatCharacterCommand command) {
            var characterInventory = inventoryFactory.createCharacterInventory();
            var character = CharacterAggregate.createFrom(command, characterInventory, combatCapabilitiesFactory);

            foreach (var itemToEquip in command.itemsToEquip) character.equipItemOrThrow(itemToEquip);

            return character;
        }
    }
}