namespace Ais.GameEngine.Core.Abstractions;

public abstract class GameLoopState : IGameLoopState
{
    public virtual Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }
}