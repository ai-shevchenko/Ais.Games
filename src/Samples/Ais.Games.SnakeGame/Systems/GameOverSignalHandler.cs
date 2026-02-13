using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Events;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class GameOverSignalHandler : EcsSystem, IInitialize
{
    private readonly IGameContextAccessor _accessor;
    private readonly IGameLoopEventBus _eventBus;
    private readonly GameSession _session;
    private IDisposable? _subscription;

    public GameOverSignalHandler(GameSession session, IGameLoopEventBus eventBus, IGameContextAccessor accessor)
    {
        _session = session;
        _eventBus = eventBus;
        _accessor = accessor;
    }

    public void Initialize()
    {
        _subscription = _eventBus.Subscribe<GameOverEvent>(null, HandleGameOver);
    }

    private Task HandleGameOver(GameOverEvent gameOverEvent, CancellationToken cancellationToken)
    {
        _session.SetResult(gameOverEvent.IsWin ? GameState.Won : GameState.Lost);
        return Task.CompletedTask;
    }

    public override void Shutdown()
    {
        _subscription?.Dispose();
        base.Shutdown();
    }
}
