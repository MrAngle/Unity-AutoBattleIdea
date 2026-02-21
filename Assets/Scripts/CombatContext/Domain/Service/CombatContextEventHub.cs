using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api.Event;
using MageFactory.Shared.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Domain.Service {
    internal sealed class CombatContextEventHub : ICombatContextEventPublisher, ICombatContextEventRegistry {
        private readonly DomainEventChannel<CombatCharacterCreatedDtoEvent, ICombatCharacterCreatedEventListener>
            combatCharacterCreatedChannel
                = new();

        public void subscribe(ICombatCharacterCreatedEventListener eventListener) {
            combatCharacterCreatedChannel.subscribe(eventListener);
        }

        public void unsubscribe(ICombatCharacterCreatedEventListener eventListener) {
            combatCharacterCreatedChannel.unsubscribe(eventListener);
        }

        public void publish(in CombatCharacterCreatedDtoEvent ev) {
            combatCharacterCreatedChannel.publish(in ev);
        }
    }
}