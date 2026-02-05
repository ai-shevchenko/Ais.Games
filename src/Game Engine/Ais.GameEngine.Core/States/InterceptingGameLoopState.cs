using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.States;

internal sealed class InterceptingGameLoopState : IGameLoopState
{
    private readonly IGameLoopStateInterceptor _interceptor;

    public InterceptingGameLoopState(IGameLoopState innerState, IGameLoopStateInterceptor interceptor)
    {
        InnerState = innerState;
        _interceptor = interceptor;
    }

    public IGameLoopState InnerState { get; }

    public async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeEnterAsync(context, stoppingToken);
        await InnerState.EnterAsync(context, stoppingToken);
        await _interceptor.AfterEnterAsync(context, stoppingToken);
    }

    public async Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeExecuteAsync(context, stoppingToken);
        await InnerState.ExecuteAsync(context, stoppingToken);
        await _interceptor.AfterExecuteAsync(context, stoppingToken);
    }

    public async Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeExitAsync(context, stoppingToken);
        await InnerState.ExitAsync(context, stoppingToken);
        await _interceptor.AfterExitAsync(context, stoppingToken);
    }
}
