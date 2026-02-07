using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IActionContext {
        void addPower(PowerAmount damageAmount);
    }
}