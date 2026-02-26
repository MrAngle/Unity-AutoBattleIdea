using System.Runtime.CompilerServices;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterFactory {
        private readonly IInventoryFactory inventoryFactory;
        private readonly IFlowFactory flowFactory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        [Inject]
        internal CharacterFactory(
            IInventoryFactory inventoryFactory,
            IFlowFactory flowFactory,
            ICharacterEventPublisher characterEventPublisher
        ) {
            this.inventoryFactory = inventoryFactory;
            this.flowFactory = flowFactory;
            this.characterEventPublisher = characterEventPublisher;
        }

        internal CharacterAggregate createCharacter(CreateCombatCharacterCommand command) {
            var characterInventory = inventoryFactory.createCharacterInventory();
            var character =
                CharacterAggregate.createFrom(command, characterInventory, flowFactory,
                    characterEventPublisher);

            foreach (var itemToEquip in command.itemsToEquip) character.equipItemOrThrow(itemToEquip);

            return character;
        }
    }
}