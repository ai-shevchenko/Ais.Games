using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class StoppingState : GameLoopState
{
    private readonly IHooksProvider _hooksProvider;

    public StoppingState(IHooksProvider hooksProvider)
    {
        _hooksProvider = hooksProvider;
    }

    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        foreach (var destroy in _hooksProvider.GetHooks<IDestroy>(true))
        {
            destroy.OnDestroy();
        }

        var asyncDestroy = _hooksProvider
            .GetHooks<IAsyncDestroy>(true)
            .Select(h => h.OnDestroyAsync(stoppingToken));

        await Task.WhenAll(asyncDestroy);
    }
}
