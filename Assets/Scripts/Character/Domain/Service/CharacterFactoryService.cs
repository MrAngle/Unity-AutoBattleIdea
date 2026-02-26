using System.Runtime.CompilerServices;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterFactoryService : ICharacterFactory {
        private readonly ICharacterCombatCapabilitiesFactory combatCapabilitiesFactory;
        private readonly IInventoryFactory inventoryFactory;
        private readonly IFlowFactory flowFactory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        [Inject]
        internal CharacterFactoryService(
            IInventoryFactory inventoryFactory,
            ICharacterCombatCapabilitiesFactory combatCapabilitiesFactory,
            IFlowFactory flowFactory,
            ICharacterEventPublisher characterEventPublisher
        ) {
            this.inventoryFactory = inventoryFactory;
            this.combatCapabilitiesFactory = combatCapabilitiesFactory;
            this.flowFactory = flowFactory;
            this.characterEventPublisher = characterEventPublisher;
        }

        public ICombatCharacter create(CreateCombatCharacterCommand command) {
            var characterInventory = inventoryFactory.createCharacterInventory();
            var character =
                CharacterAggregate.createFrom(command, characterInventory, combatCapabilitiesFactory, flowFactory,
                    characterEventPublisher);

            foreach (var itemToEquip in command.itemsToEquip) character.equipItemOrThrow(itemToEquip);

            return character;
        }
    }
}