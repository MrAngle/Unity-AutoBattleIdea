namespace MageFactory.CombatContext.Api.Event {
    public interface ICombatContextEventPublisher {
        void publish(in CombatCharacterCreatedDtoEvent ev);
        void publish(in CombatContextCreatedDtoEvent ev);
        void publish(in FlowGuardCreatedDtoEvent ev);
        void publish(in FlowStabilityCreatedDtoEvent ev);
        void publish(in FlowInputStartedDtoEvent ev);
        void publish(in FlowOutputReachedDtoEvent ev);
        void publish(in FlowNoOutputDtoEvent ev);
        void publish(in FlowAttackCreatedDtoEvent ev);
        void publish(in DamagePacketLayerProcessedDtoEvent ev);
    }

    public interface ICombatContextEventRegistry {
        void subscribe(ICombatCharacterCreatedEventListener eventListener);
        void unsubscribe(ICombatCharacterCreatedEventListener eventListener);

        void subscribe(ICombatContextEventListener eventListener);
        void unsubscribe(ICombatContextEventListener eventListener);

        void subscribe(IFlowGuardCreatedEventListener eventListener);
        void unsubscribe(IFlowGuardCreatedEventListener eventListener);

        void subscribe(IFlowStabilityCreatedEventListener eventListener);
        void unsubscribe(IFlowStabilityCreatedEventListener eventListener);

        void subscribe(IFlowInputStartedEventListener eventListener);
        void unsubscribe(IFlowInputStartedEventListener eventListener);

        void subscribe(IFlowOutputReachedEventListener eventListener);
        void unsubscribe(IFlowOutputReachedEventListener eventListener);

        void subscribe(IFlowNoOutputEventListener eventListener);
        void unsubscribe(IFlowNoOutputEventListener eventListener);

        void subscribe(IFlowAttackCreatedEventListener eventListener);
        void unsubscribe(IFlowAttackCreatedEventListener eventListener);

        void subscribe(IDamagePacketLayerProcessedEventListener eventListener);
        void unsubscribe(IDamagePacketLayerProcessedEventListener eventListener);
    }
}