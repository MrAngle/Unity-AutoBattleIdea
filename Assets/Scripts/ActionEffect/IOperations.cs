using System.Collections.Generic;

namespace MageFactory.ActionEffect {
    public interface IOperations {
        IReadOnlyList<IOperation> getEffects();
    }
}