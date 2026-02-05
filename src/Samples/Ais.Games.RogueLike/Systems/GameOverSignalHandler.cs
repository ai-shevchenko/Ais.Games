using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

/// <summary>
/// Асинхронный обработчик сигнала окончания игры.
/// Демонстрирует использование SignalBus и async/await.
/// </summary>
internal sealed class GameOverSignalHandler : EcsSystem, IAsyncInitialize
{
    private readonly ISignalSubscriber _subscriber;
    private readonly GameSession _session;
    private IDisposable? _subscription;

    public GameOverSignalHandler(ISignalSubscriber subscriber, GameSession session)
    {
        _subscriber = subscriber;
        _session = session;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _subscription = _subscriber.Subscribe<GameOverSignal>(async signal =>
        {
            // Имитация асинхронного сохранения результата (например, в файл/БД)
            await Task.Delay(200, cancellationToken);

            _session.SetResult(signal.IsWin ? GameResult.Won : GameResult.Lost);
        });

        await Task.CompletedTask;
    }

    public override void Shutdown()
    {
        _subscription?.Dispose();
        base.Shutdown();
    }
}

