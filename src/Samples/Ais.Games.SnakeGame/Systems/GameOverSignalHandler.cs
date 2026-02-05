using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class GameOverSignalHandler : EcsSystem, IAsyncInitialize
{
    private readonly GameSession _session;
    private readonly ISignalSubscriber _subscriber;
    private IDisposable? _subscription;

    public GameOverSignalHandler(ISignalSubscriber subscriber, GameSession session)
    {
        _subscriber = subscriber;
        _session = session;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _subscription = _subscriber.Subscribe<GameOverSignal>(signal =>
        {
            _session.SetResult(signal.IsWin ? GameState.Won : GameState.Lost);
        });

        await Task.CompletedTask;
    }

    public override void Shutdown()
    {
        _subscription?.Dispose();
        base.Shutdown();
    }
}
