using System.Threading;
using System.Threading.Tasks;

namespace MageFactory.Flow.Domain {
    /// <summary>
    /// Abstrakcja "oddania sterowania" pomiędzy porcjami pracy flow.
    /// Runtime może implementować to jako "kolejna klatka".
    /// Testy mogą użyć implementacji natychmiastowej.
    /// </summary>
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