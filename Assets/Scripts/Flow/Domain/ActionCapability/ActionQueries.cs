using MageFactory.ActionEffect;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionQueries : IActionQueries {
        private readonly ActionContext actionContext;
        private readonly IFlowQueries flowQueries;

        public ActionQueries(ActionContext actionContext, IFlowQueries flowQueries) {
            this.actionContext = NullGuard.NotNullOrThrow(actionContext);
            this.flowQueries = NullGuard.NotNullOrThrow(flowQueries);
        }
    }
}