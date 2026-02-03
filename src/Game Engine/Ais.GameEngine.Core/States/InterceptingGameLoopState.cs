using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class InterceptingGameLoopState : IGameLoopState
{
    private readonly IGameLoopState _innerState;
    private readonly IGameLoopStateInterceptor _interceptor;

    public InterceptingGameLoopState(IGameLoopState innerState, IGameLoopStateInterceptor interceptor)
    {
        _innerState = innerState;
        _interceptor = interceptor;
    }

    public async Task EnterAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeEnterAsync(context, stoppingToken);
        await _innerState.EnterAsync(context, stoppingToken);
        await _interceptor.AfterEnterAsync(context, stoppingToken);
    }

    public async Task ExecuteAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeExecuteAsync(context, stoppingToken);
        await _innerState.ExecuteAsync(context, stoppingToken);
        await _interceptor.AfterExecuteAsync(context, stoppingToken);
    }

    public async Task ExitAsync(GameLoopContext context, CancellationToken stoppingToken = default)
    {
        await _interceptor.BeforeExitAsync(context, stoppingToken);
        await _innerState.ExitAsync(context, stoppingToken);
        await _interceptor.AfterExitAsync(context, stoppingToken);
    }
}
