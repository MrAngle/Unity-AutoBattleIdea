using Contracts.Character;
using Shared.Utility;
using Zenject;

namespace Context {
    public class CharacterAggregateContext {
        private readonly InventoryAggregateContext _inventoryAggregateContext;

        private ICharacterAggregateFacade _characterAggregateFacade;

        [Inject]
        public CharacterAggregateContext(InventoryAggregateContext inventoryAggregateContext) {
            _inventoryAggregateContext = NullGuard.NotNullOrThrow(inventoryAggregateContext);
        }

        public void SetCharacterAggregateContext(ICharacterAggregateFacade characterAggregateFacade) {
            _characterAggregateFacade = characterAggregateFacade;
            _inventoryAggregateContext.SetInventoryAggregateContext(_characterAggregateFacade.GetInventoryAggregate());
        }

        public ICharacterAggregateFacade GetCharacterAggregateContext() {
            return _characterAggregateFacade;
        }
    }
}