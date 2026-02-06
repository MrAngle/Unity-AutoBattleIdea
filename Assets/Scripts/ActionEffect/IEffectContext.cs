using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IEffectContext {
        void addPower(PowerAmount damageAmount);
    }
}