using Contracts.Character;
using Shared.Utility;
using Zenject;

namespace Context {
    public class CharacterAggregateContext {
        private readonly InventoryAggregateContext _inventoryAggregateContext;

        private ICharacter _character;

        [Inject]
        public CharacterAggregateContext(InventoryAggregateContext inventoryAggregateContext) {
            _inventoryAggregateContext = NullGuard.NotNullOrThrow(inventoryAggregateContext);
        }

        public void SetCharacterAggregateContext(ICharacter character) {
            _character = character;
            _inventoryAggregateContext.SetInventoryAggregateContext(_character.getInventoryAggregate());
        }

        public ICharacter GetCharacterAggregateContext() {
            return _character;
        }
    }
}