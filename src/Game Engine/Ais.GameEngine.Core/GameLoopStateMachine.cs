using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Interceptors;
using Ais.GameEngine.Core.States;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateMachine : IGameLoopStateMachine
{
    private readonly IGameLoopContextAccessor _contextAccessor;
    private readonly ILogger<GameLoopStateMachine> _logger;
    private readonly IGameLoopStateProvider _stateProvider;
    private readonly IEnumerable<IGameLoopStateInterceptor> _interceptors;

    private bool _disposed;
    private CancellationTokenSource? _executionCts;
    private Task? _executionTask;
    private bool _isRunning;

    public GameLoopStateMachine(
        IGameLoopStateProvider stateProvider,
        IGameLoopContextAccessor contextAccessor,
        ILogger<GameLoopStateMachine> logger,
        IEnumerable<IGameLoopStateInterceptor> interceptors)
    {
        _stateProvider = stateProvider;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _interceptors = interceptors;
    }

    public IGameLoopState? CurrentState => _contextAccessor.CurrentContext?.CurrentState;

    public async Task ChangeStateAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var newState = GetState<T>();

        if (_contextAccessor.CurrentContext is { CurrentState: not null })
        {
            await _contextAccessor.CurrentContext.CurrentState.ExitAsync(_contextAccessor.CurrentContext, stoppingToken);
        }

        var previousState = _contextAccessor.CurrentState;
        _contextAccessor.CurrentState = newState;

        try
        {
            await newState.EnterAsync(_contextAccessor, stoppingToken);
        }
        catch (Exception ex)
        {
            _contextAccessor.CurrentState = previousState;
            throw new StateTransitionException($"Failed to enter state {typeof(T).Name}", ex);
        }
    }

    public async Task StartAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_isRunning)
        {
            return;
        }

        await ChangeStateAsync<T>(stoppingToken);
        _isRunning = true;

        _executionCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _executionTask = Task.Run(async () =>
        {
            while (_isRunning && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (CurrentState is not null)
                    {
                        await CurrentState.ExecuteAsync(_contextAccessor, stoppingToken);
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was canceled cause {@Reason}",
                        _contextAccessor.LoopName, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was failed cause {@Reason}",
                        _contextAccessor.LoopName, ex.Message);
                    throw;
                }
            }
        }, _executionCts.Token);

        await _executionTask;
    }

    public async Task StopAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _executionCts?.CancelAsync();

        if (CurrentState is not null)
        {
            await CurrentState.ExitAsync(_contextAccessor);
        }

        if (_executionTask is not null)
        {
            await Task.WhenAny(_executionTask);
        }
    }

    private IGameLoopState GetState<T>() where T : IGameLoopState
    {
        if (!_interceptors.Any())
        {
            return _stateProvider.GetState<T>();
        }

        var innerState = _stateProvider.GetState<T>();
        return new InterceptingGameLoopState(innerState, new CompositeInterceptor(_interceptors));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopAsync().Wait();

        _disposed = true;
    }
}
