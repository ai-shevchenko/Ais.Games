namespace Ais.GameEngine.StateMachine.Abstractions;

public abstract class GameStateInterceptorBase : IGameStateInterceptor
{
    public virtual Task BeforeEnterAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterEnterAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task BeforeExecuteAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterExecuteAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task BeforeExitAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterExitAsync(GameContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(GameContext context, Exception exception, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
