using MageFactory.Character.Api;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class CharacterAggregateContext {
        private readonly InventoryAggregateContext _inventoryAggregateContext;

        private ICharacter _character;

        [Inject]
        public CharacterAggregateContext(InventoryAggregateContext inventoryAggregateContext) {
            _inventoryAggregateContext = NullGuard.NotNullOrThrow(inventoryAggregateContext);
        }

        public void setCharacterAggregateContext(ICharacter character) {
            _character = character;
            _inventoryAggregateContext.setInventoryAggregateContext(_character.getInventoryAggregate());
        }

        public ICharacter getCharacterAggregateContext() {
            return _character;
        }
    }
}