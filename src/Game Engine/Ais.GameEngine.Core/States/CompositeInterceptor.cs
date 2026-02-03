using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.States;

public sealed class CompositeInterceptor : IGameLoopStateInterceptor
{
    private readonly IEnumerable<IGameLoopStateInterceptor> _interceptors;

    public CompositeInterceptor(IEnumerable<IGameLoopStateInterceptor> interceptors)
    {
        _interceptors = interceptors;
    }

    public async Task BeforeEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeEnterAsync(context, stoppingToken);
        }
    }

    public async Task AfterEnterAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterEnterAsync(context, stoppingToken);
        }
    }

    public async Task BeforeExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeExecuteAsync(context, stoppingToken);
        }
    }

    public async Task AfterExecuteAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterExecuteAsync(context, stoppingToken);
        }
    }

    public async Task BeforeExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeExitAsync(context, stoppingToken);
        }
    }

    public async Task AfterExitAsync(GameLoopContext context, CancellationToken stoppingToken)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterExitAsync(context, stoppingToken);
        }
    }
}