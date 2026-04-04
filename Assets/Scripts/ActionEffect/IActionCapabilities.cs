using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IActionCapabilities {
        IActionCommandBus command();
        IActionQueries query();
    }

    public interface IActionCommandBus {
        void addPower(PowerAmount damageAmount);

        void pushRightAdjacentItemRight();
    }

    public interface IActionQueries {
    }
}