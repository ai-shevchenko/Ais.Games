using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.States;

public sealed class StoppingState : GameLoopState
{
    private readonly ILogger<StoppingState> _logger;

    public StoppingState(ILogger<StoppingState> logger)
    {
        _logger = logger;
    }

    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Stopping game loop {@LoopName}", context.LoopName);
        }
        
        foreach (var destroy in context.Hooks.GetHooks<IDestroy>(enabledOnly: true))
        {
            destroy.OnDestroy();
        }

        var asyncDestroy = context.Hooks
            .GetHooks<IAsyncDestroy>(enabledOnly: true)
            .Select(h => h.OnDestroyAsync(stoppingToken));

        await Task.WhenAll(asyncDestroy);
    }

    public override Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("The game loop {@LoopName} wa stopped", context.LoopName);
        }
        
        return base.ExitAsync(context, stoppingToken);
    }
}
