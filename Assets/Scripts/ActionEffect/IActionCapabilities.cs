using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IActionCapabilities {
        IActionCommandBus command();
        IActionQueries query();
    }

    public interface IActionCommandBus {
        void addPower(DamageRole damageRole, PowerAmount damageAmount);

        void pushRightAdjacentItemRight();
    }

    public interface IActionQueries {
    }
}