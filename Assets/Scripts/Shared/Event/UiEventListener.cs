namespace MageFactory.Shared.Event {
    public interface IUiEvent : IMageFactoryEvent {
    }

    public interface IUiEventListener<TEvent> : IMageEventListener<TEvent> where TEvent : IUiEvent {
    }
}