using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MageFactory.Tests")]

namespace MageFactory.Flow.Domain {
    internal interface IFlowStepScheduler {
        Task yieldAsync(CancellationToken ct);
    }

    internal sealed class ImmediateFlowStepScheduler : IFlowStepScheduler {
        public Task yieldAsync(CancellationToken ct) => Task.CompletedTask;
    }

    internal sealed class TaskYieldFlowStepScheduler : IFlowStepScheduler {
        public async Task yieldAsync(CancellationToken ct) {
            ct.ThrowIfCancellationRequested();
            await Task.Yield();
        }
    }
}