using MageFactory.ActionEffect;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionCapabilities : IActionCapabilities {
        private readonly ActionCommandBus actionCommandBus;
        private readonly ActionQueries actionQueries;

        public ActionCapabilities(ActionContext actionContext) {
            actionCommandBus = new ActionCommandBus(actionContext);
            actionQueries = new ActionQueries(actionContext);
        }

        public IActionCommandBus command() {
            return actionCommandBus;
        }

        public IActionQueries query() {
            return actionQueries;
        }
    }
}