using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public interface IActionDescription {
        public ItemCastTime getCastTime();
        public IOperations getEffectsDescriptor();
    }
}