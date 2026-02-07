using MageFactory.Shared.Model;

namespace MageFactory.ActionExecutor.Api.Dto {
    public interface IActionDescription {
        public Duration getCastTime();
        public IOperations getEffectsDescriptor();
    }
}