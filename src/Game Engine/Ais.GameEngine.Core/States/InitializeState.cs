using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.States;

public sealed class InitializeState : GameLoopState
{
    private readonly ILogger<InitializeState> _logger;
    private readonly IGameLoopStateMachine _stateMachine;

    public InitializeState(IGameLoopStateMachine stateMachine, ILogger<InitializeState> logger)
    {
        _stateMachine = stateMachine;
        _logger = logger;
    }

    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Start initializing game loop {@LoopName}", context.LoopName);
        }

        foreach (var hook in context.Hooks.GetHooks<IInitialize>())
        {
            hook.Initialize();
        }

        var asyncHooks = context.Hooks
            .GetHooks<IAsyncInitialize>()
            .Select(h => h.InitializeAsync(stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    public override async Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _stateMachine.ChangeStateAsync<RunningState>(stoppingToken);
    }

    public override Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Initializing of game loop {@LoopName} finished", context.LoopName);
        }

        return base.ExitAsync(context, stoppingToken);
    }
}
