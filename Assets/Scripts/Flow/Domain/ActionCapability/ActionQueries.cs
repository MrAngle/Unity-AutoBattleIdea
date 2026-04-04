using MageFactory.ActionEffect;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionQueries : IActionQueries {
        private readonly ActionContext actionContext;

        public ActionQueries(ActionContext actionContext) {
            this.actionContext = NullGuard.NotNullOrThrow(actionContext);
        }
    }
}