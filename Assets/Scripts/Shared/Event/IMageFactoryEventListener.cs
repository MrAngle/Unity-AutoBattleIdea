namespace MageFactory.Shared.Event {
    public interface IMageFactoryEvent {
    }

    public interface IMageEventListener<TEvent>
        where TEvent : IMageFactoryEvent {
        void onEvent(in TEvent ev);
    }
}