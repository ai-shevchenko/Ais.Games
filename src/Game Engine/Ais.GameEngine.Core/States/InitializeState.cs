using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class InitializeState : GameLoopState
{
    private readonly IGameLoopStateMachine _stateMachine;

    public InitializeState(IGameLoopStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
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
}
