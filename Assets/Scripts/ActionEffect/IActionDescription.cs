using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IActionDescription {
        public Duration getCastTime();
        public IOperations getEffectsDescriptor();
    }
}