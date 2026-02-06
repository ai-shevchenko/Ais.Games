namespace Ais.GameEngine.StateMachine.Abstractions;

public abstract class GameStateBase : IGameState
{
    public virtual Task EnterAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task ExecuteAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task ExitAsync(GameContext context, CancellationToken stoppingToken = default)
    {
        return Task.CompletedTask;
    }
}
