using System.Collections.Generic;
using MageFactory.ActionEffect;

namespace MageFactory.ActionExecutor.Api.Dto {
    public interface IOperations {
        IReadOnlyList<IOperation> getEffects();
    }
}