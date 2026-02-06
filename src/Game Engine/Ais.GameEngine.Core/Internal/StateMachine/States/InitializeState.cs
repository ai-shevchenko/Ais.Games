using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Internal.StateMachine.States;

internal sealed class InitializeState : GameStateBase
{
    private readonly IHooksProvider _hooksProvider;
    private readonly IGameStateMachine _stateMachine;

    public InitializeState(IGameStateMachine stateMachine, IHooksProvider hooksProvider)
    {
        _stateMachine = stateMachine;
        _hooksProvider = hooksProvider;
    }

    public override async Task EnterAsync(GameContext context, CancellationToken stoppingToken = default)
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

    public override async Task ExecuteAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        await _stateMachine.ChangeStateAsync<RunningState>(stoppingToken);
    }
}
