using Ais.GameEngine.Core.States;
using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Interceptors;

public abstract class GameLoopStateInterceptor : IGameLoopStateInterceptor
{
    public virtual Task BeforeEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task BeforeExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task BeforeExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public IGameLoopState? GetInterceptedState(GameLoopContext context)
    {
        return (context.CurrentState as InterceptingGameLoopState)?.InnerState;
    }
}
