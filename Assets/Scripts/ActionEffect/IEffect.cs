namespace MageFactory.ActionEffect {
    public interface IEffect {
        // IEffectInstance Bind(IFlow flowAggregate);
        void apply(IEffectContext effectContext);
    }
}