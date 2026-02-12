using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class GameOverSignalHandler : EcsSystem, IAsyncInitialize
{
    private readonly GameSession _session;
    private IDisposable? _subscription;

    public GameOverSignalHandler(GameSession session)
    {
        _session = session;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public override void Shutdown()
    {
        _subscription?.Dispose();
        base.Shutdown();
    }
}
