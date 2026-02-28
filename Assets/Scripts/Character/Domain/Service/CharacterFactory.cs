using System.Runtime.CompilerServices;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract.Command;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Character.Domain.Service {
    internal class CharacterFactory {
        private readonly ICharacterInventoryFactory characterInventoryFactory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        [Inject]
        internal CharacterFactory(
            ICharacterInventoryFactory characterInventoryFactory,
            ICharacterEventPublisher characterEventPublisher
        ) {
            this.characterInventoryFactory = characterInventoryFactory;
            this.characterEventPublisher = characterEventPublisher;
        }

        internal CharacterAggregate createCharacter(CreateCombatCharacterCommand command) {
            var characterInventory = characterInventoryFactory.createCharacterInventory();
            var character =
                CharacterAggregate.createFrom(command, characterInventory, characterEventPublisher);

            foreach (var itemToEquip in command.itemsToEquip) {
                character.equipItemOrThrow(itemToEquip);
            }

            return character;
        }
    }
}