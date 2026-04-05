using MageFactory.ActionEffect;
using MageFactory.Flow.Contract;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionCapabilities : IActionCapabilities {
        private readonly ActionCommandBus actionCommandBus;
        private readonly ActionQueries actionQueries;

        public ActionCapabilities(ActionContext actionContext, IFlowCapabilities flowCapabilities) {
            actionCommandBus = new ActionCommandBus(actionContext, flowCapabilities);
            actionQueries = new ActionQueries(actionContext, flowCapabilities.query());
        }

        public IActionCommandBus command() {
            return actionCommandBus;
        }

        public IActionQueries query() {
            return actionQueries;
        }
    }
}