using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class CharacterAggregateContext {
        private readonly InventoryAggregateContext _inventoryAggregateContext;
        private ICombatCharacter _character;

        [Inject]
        public CharacterAggregateContext(InventoryAggregateContext inventoryAggregateContext) {
            _inventoryAggregateContext = NullGuard.NotNullOrThrow(inventoryAggregateContext);
        }

        public void setCharacterAggregateContext(ICombatCharacter character) {
            _character = character;
            _inventoryAggregateContext.setInventoryAggregateContext(_character.getInventoryAggregate());
        }

        public ICombatCharacter getCharacterAggregateContext() {
            return _character;
        }
    }
}