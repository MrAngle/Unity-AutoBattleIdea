using Character;

namespace Context {
    
    public static class CharacterAggregateContext {
        private static ICharacterAggregateFacade _characterAggregateFacade;

        // [Inject]
        // public InventoryAggregateContext(IInventoryAggregateFactory inventoryAggregateFactory) {
        //     _characterAggregateFacade = inventoryAggregateFactory.CreateCharacterInventory();
        // }
        
        public static void SetCharacterAggregateContext(ICharacterAggregateFacade characterAggregateFacade) {
            _characterAggregateFacade = characterAggregateFacade;
        }

        public static ICharacterAggregateFacade GetCharacterAggregateContext() {
            return _characterAggregateFacade;
        }
    }
}