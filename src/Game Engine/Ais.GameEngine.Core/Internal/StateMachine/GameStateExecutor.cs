using Ais.GameEngine.StateMachine.Abstractions;

namespace Ais.GameEngine.Core.Internal.StateMachine;

internal sealed class GameStateExecutor : IGameStateExecutor
{
    private readonly IGameContextAccessor _contextAccessor;
    private readonly IEnumerable<IGameStateInterceptor> _interceptors;

    public GameStateExecutor(IEnumerable<IGameStateInterceptor> interceptors, IGameContextAccessor contextAccessor)
    {
        _interceptors = interceptors;
        _contextAccessor = contextAccessor;
    }

    public async Task EnterAsync(IGameState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        if (!_interceptors.Any())
        {
            await state.EnterAsync(_contextAccessor.CurrentContext, cancellationToken);
            return;
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeEnterAsync(_contextAccessor.CurrentContext, cancellationToken);
        }

        try
        {
            await state.EnterAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
        catch (Exception ex)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnErrorAsync(_contextAccessor.CurrentContext, ex, cancellationToken);
            }
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterEnterAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
    }

    public async Task ExecuteAsync(IGameState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        if (!_interceptors.Any())
        {
            await state.ExecuteAsync(_contextAccessor.CurrentContext, cancellationToken);
            return;
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeExecuteAsync(_contextAccessor.CurrentContext, cancellationToken);
        }

        try
        {
            await state.ExecuteAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
        catch (Exception ex)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnErrorAsync(_contextAccessor.CurrentContext, ex, cancellationToken);
            }
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterExecuteAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
    }

    public async Task ExitAsync(IGameState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        if (!_interceptors.Any())
        {
            await state.ExitAsync(_contextAccessor.CurrentContext, cancellationToken);
            return;
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.BeforeExitAsync(_contextAccessor.CurrentContext, cancellationToken);
        }

        try
        {
            await state.ExitAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
        catch (Exception ex)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnErrorAsync(_contextAccessor.CurrentContext, ex, cancellationToken);
            }
        }

        foreach (var interceptor in _interceptors)
        {
            await interceptor.AfterExitAsync(_contextAccessor.CurrentContext, cancellationToken);
        }
    }
}
