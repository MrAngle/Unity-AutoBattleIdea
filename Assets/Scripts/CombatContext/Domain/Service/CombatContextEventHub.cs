using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api.Event;
using MageFactory.Shared.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Domain.Service {
    internal sealed class CombatContextEventHub : ICombatContextEventPublisher, ICombatContextEventRegistry {
        private readonly DomainEventChannel<CombatCharacterCreatedDtoEvent, ICombatCharacterCreatedEventListener>
            combatCharacterCreatedChannel
                = new();

        private readonly DomainEventChannel<CombatContextCreatedDtoEvent, ICombatContextEventListener>
            combatContextCreatedChannel
                = new();

        private readonly DomainEventChannel<FlowGuardCreatedDtoEvent, IFlowGuardCreatedEventListener>
            flowGuardCreatedChannel = new();

        private readonly DomainEventChannel<FlowStabilityCreatedDtoEvent, IFlowStabilityCreatedEventListener>
            flowStabilityCreatedChannel = new();

        private readonly DomainEventChannel<FlowInputStartedDtoEvent, IFlowInputStartedEventListener>
            flowInputStartedChannel = new();

        private readonly DomainEventChannel<FlowOutputReachedDtoEvent, IFlowOutputReachedEventListener>
            flowOutputReachedChannel = new();

        private readonly DomainEventChannel<FlowNoOutputDtoEvent, IFlowNoOutputEventListener>
            flowNoOutputChannel = new();

        private readonly DomainEventChannel<FlowAttackCreatedDtoEvent, IFlowAttackCreatedEventListener>
            flowAttackCreatedChannel = new();

        private readonly DomainEventChannel<DamagePacketLayerProcessedDtoEvent,
                IDamagePacketLayerProcessedEventListener>
            damagePacketLayerProcessedChannel = new();

        public void subscribe(ICombatCharacterCreatedEventListener eventListener) {
            combatCharacterCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(ICombatCharacterCreatedEventListener eventListener) {
            combatCharacterCreatedChannel.unsubscribe(eventListener);
        }

        public void subscribe(ICombatContextEventListener eventListener) {
            combatContextCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(ICombatContextEventListener eventListener) {
            combatContextCreatedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowGuardCreatedEventListener eventListener) {
            flowGuardCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowGuardCreatedEventListener eventListener) {
            flowGuardCreatedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowStabilityCreatedEventListener eventListener) {
            flowStabilityCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowStabilityCreatedEventListener eventListener) {
            flowStabilityCreatedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowInputStartedEventListener eventListener) {
            flowInputStartedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowInputStartedEventListener eventListener) {
            flowInputStartedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowOutputReachedEventListener eventListener) {
            flowOutputReachedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowOutputReachedEventListener eventListener) {
            flowOutputReachedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowNoOutputEventListener eventListener) {
            flowNoOutputChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowNoOutputEventListener eventListener) {
            flowNoOutputChannel.unsubscribe(eventListener);
        }

        public void subscribe(IFlowAttackCreatedEventListener eventListener) {
            flowAttackCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IFlowAttackCreatedEventListener eventListener) {
            flowAttackCreatedChannel.unsubscribe(eventListener);
        }

        public void subscribe(IDamagePacketLayerProcessedEventListener eventListener) {
            damagePacketLayerProcessedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IDamagePacketLayerProcessedEventListener eventListener) {
            damagePacketLayerProcessedChannel.unsubscribe(eventListener);
        }

        public void publish(in CombatCharacterCreatedDtoEvent ev) {
            combatCharacterCreatedChannel.publish(in ev);
        }

        public void publish(in CombatContextCreatedDtoEvent ev) {
            combatContextCreatedChannel.publish(in ev);
        }

        public void publish(in FlowGuardCreatedDtoEvent ev) {
            flowGuardCreatedChannel.publish(in ev);
        }

        public void publish(in FlowStabilityCreatedDtoEvent ev) {
            flowStabilityCreatedChannel.publish(in ev);
        }

        public void publish(in FlowInputStartedDtoEvent ev) {
            flowInputStartedChannel.publish(in ev);
        }

        public void publish(in FlowOutputReachedDtoEvent ev) {
            flowOutputReachedChannel.publish(in ev);
        }

        public void publish(in FlowNoOutputDtoEvent ev) {
            flowNoOutputChannel.publish(in ev);
        }

        public void publish(in FlowAttackCreatedDtoEvent ev) {
            flowAttackCreatedChannel.publish(in ev);
        }

        public void publish(in DamagePacketLayerProcessedDtoEvent ev) {
            damagePacketLayerProcessedChannel.publish(in ev);
        }
    }
}