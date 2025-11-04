using System;
using System.Threading;
using System.Threading.Tasks;
using TimeSystem;
using UnityEngine;
using Combat.Flow.Domain.Aggregate;
using Inventory.Items.Domain;

namespace Combat.ActionExecutor
{


    public interface IActionExecutor
    {
        Task ExecuteAsync(IPreparedAction preparedAction, CancellationToken cancellationToken = default);        
    }

    public sealed class ActionExecutor : IActionExecutor
    {
        public async Task ExecuteAsync(IPreparedAction preparedAction, CancellationToken cancellationToken = default)
        {
            // var spd = Mathf.Max(0.001f, speedMultiplier);
            // var duration = action.BaseDurationSeconds / spd;
            var duration = preparedAction.GetActionTiming().DurationSeconds();

            await TimeModule.ContinueIn(duration, cancellationToken);
            preparedAction.Execute();
        }
    }
    


}