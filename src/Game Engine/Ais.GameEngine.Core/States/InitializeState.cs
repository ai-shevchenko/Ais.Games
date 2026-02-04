using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class InitializeState : GameLoopState
{
    private readonly IHooksProvider _hooksProvider;
    private readonly IGameLoopStateMachine _stateMachine;

    public InitializeState(IGameLoopStateMachine stateMachine, IHooksProvider hooksProvider)
    {
        _stateMachine = stateMachine;
        _hooksProvider = hooksProvider;
    }

    public override async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        foreach (var hook in _hooksProvider.GetHooks<IInitialize>())
        {
            hook.Initialize();
        }

        var asyncHooks = _hooksProvider
            .GetHooks<IAsyncInitialize>()
            .Select(h => h.InitializeAsync(stoppingToken));

        await Task.WhenAll(asyncHooks);
    }

    public override async Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _stateMachine.ChangeStateAsync<RunningState>(stoppingToken);
    }
}
