using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class StoppingState : GameLoopState
{
    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        foreach (var destroy in context.Hooks.GetHooks<IDestroy>(true))
        {
            destroy.OnDestroy();
        }

        var asyncDestroy = context.Hooks
            .GetHooks<IAsyncDestroy>(true)
            .Select(h => h.OnDestroyAsync(stoppingToken));

        await Task.WhenAll(asyncDestroy);
    }
}
