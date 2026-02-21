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

        public void publish(in CombatCharacterCreatedDtoEvent ev) {
            combatCharacterCreatedChannel.publish(in ev);
        }

        public void publish(in CombatContextCreatedDtoEvent ev) {
            combatContextCreatedChannel.publish(in ev);
        }
    }
}