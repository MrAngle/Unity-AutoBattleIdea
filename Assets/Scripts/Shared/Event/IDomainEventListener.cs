namespace MageFactory.Shared.Event {
    public interface IDomainEvent : IMageFactoryEvent {
    }

    public interface IDomainEventListener<TEvent> : IMageEventListener<TEvent> where TEvent : IDomainEvent {
    }
}