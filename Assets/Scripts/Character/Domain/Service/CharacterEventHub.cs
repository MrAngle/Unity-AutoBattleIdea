using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.Shared.Event;

namespace MageFactory.Character.Domain.Service {
    internal sealed class CharacterEventHub : ICharacterEventPublisher, ICharacterEventRegistry {
        private readonly DomainEventChannel<CharacterHpChangedDtoEvent, IHpChangedEventListener>
            characterHpChangedChannel =
                new();

        private readonly DomainEventChannel<CharacterDeathDtoEvent, ICharacterDeathEventListener>
            characterDeathChannel = new();

        public void subscribe(IHpChangedEventListener eventListener) =>
            characterHpChangedChannel.subscribe(eventListener);

        public void unsubscribe(IHpChangedEventListener eventListener) =>
            characterHpChangedChannel.unsubscribe(eventListener);

        public void subscribe(ICharacterDeathEventListener eventListener) =>
            characterDeathChannel.subscribe(eventListener);

        public void unsubscribe(ICharacterDeathEventListener eventListener) =>
            characterDeathChannel.unsubscribe(eventListener);

        public void publish(in CharacterHpChangedDtoEvent ev) =>
            characterHpChangedChannel.publish(in ev);

        public void publish(in CharacterDeathDtoEvent ev) =>
            characterDeathChannel.publish(in ev);
    }

    // old version to check how handle events by id
    // internal sealed class CharacterEventHub : ICharacterEventPublisher, ICharacterEventRegistry {
    //     private readonly Dictionary<Id<CharacterId>, DomainEventChannel<HpChangedDtoEvent, IHpChangedEventListener>> 
    //         characterHpChangedChannel = new();
    //     private readonly Dictionary<Id<CharacterId>, DomainEventChannel<CharacterDeathDtoEvent, ICharacterDeathEventListener>> 
    //         characterDeathChannel = new();
    //
    //     private static DomainEventChannel<TEvent, TListener> getOrCreateChannel<TKey, TEvent, TListener>(
    //         TKey id,
    //         Dictionary<TKey, DomainEventChannel<TEvent, TListener>> channels
    //     )
    //         where TEvent : IDomainEvent
    //         where TListener : class, IDomainEventListener<TEvent>
    //     {
    //         if (!channels.TryGetValue(id, out var channel)) {
    //             channel = new DomainEventChannel<TEvent, TListener>();
    //             channels[id] = channel;
    //         }
    //
    //         return channel;
    //     }
    //     
    //     // private <T> DomainEventChannel<HpChangedDtoEvent, IDomainEventListener> getOrCreateChannel(
    //     //     Id<CharacterId> id, 
    //     //     Dictionary<Id<CharacterId>, DomainEventChannel<HpChangedDtoEvent, IDomainEventListener<T>>> channels) {
    //     //     if (!channels.TryGetValue(id, out var channel)) {
    //     //         channel = new DomainEventChannel<HpChangedDtoEvent, IHpChangedEventListener>();
    //     //         channels[id] = channel;
    //     //     }
    //     //     return channel;
    //     // }
    //
    //     public void subscribe(Id<CharacterId> characterId, IHpChangedEventListener listener) {
    //         getOrCreateChannel(characterId, characterHpChangedChannel).subscribe(listener);
    //     }
    //
    //     public void unsubscribe(Id<CharacterId> characterId, IHpChangedEventListener listener) {
    //         if (characterHpChangedChannel.TryGetValue(characterId, out var channel)) {
    //             channel.unsubscribe(listener);
    //         }
    //     }
    //
    //     public void subscribe(Id<CharacterId> characterId, ICharacterDeathEventListener eventListener) {
    //         getOrCreateChannel(characterId, characterDeathChannel).subscribe(eventListener);
    //     }
    //
    //     public void unsubscribe(Id<CharacterId> characterId, ICharacterDeathEventListener eventListener) {
    //         if (characterDeathChannel.TryGetValue(characterId, out var channel)) {
    //             channel.unsubscribe(eventListener);
    //         }
    //     }
    //
    //     public void publish(in HpChangedDtoEvent ev) {
    //         if (characterHpChangedChannel.TryGetValue(ev.characterId, out var channel)) {
    //             channel.publish(ev);
    //         }
    //     }
    //
    //     public void publish(in CharacterDeathDtoEvent ev) {
    //         if (characterDeathChannel.TryGetValue(ev.characterId, out var channel)) {
    //             channel.publish(ev);
    //         }
    //     }
    // }
}