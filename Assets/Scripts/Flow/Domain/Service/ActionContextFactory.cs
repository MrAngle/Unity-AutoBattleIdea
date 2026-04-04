using System.Runtime.CompilerServices;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.ActionCapability;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Flow.Domain.Service {
    internal sealed class ActionContextFactory {
        private readonly SignalBus signalBus;

        [Inject]
        public ActionContextFactory(SignalBus injectSignalBus) {
            signalBus = injectSignalBus;
        }

        internal ActionContext create(FlowContext flowContext, IFlowItem actionItemInvoker) {
            return new ActionContext(flowContext, actionItemInvoker, signalBus);
        }
    }
}