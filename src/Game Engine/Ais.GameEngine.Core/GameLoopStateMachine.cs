using Ais.GameEngine.Core.Interceptors;
using Ais.GameEngine.Core.States;
using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateMachine : IGameLoopStateMachine
{
    private readonly IGameLoopContextAccessor _contextAccessor;
    private readonly IEnumerable<IGameLoopStateInterceptor> _interceptors;
    private readonly ILogger<GameLoopStateMachine> _logger;
    private readonly IGameLoopStateProvider _stateProvider;

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
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        var newState = GetState<T>();

        if (_contextAccessor.CurrentContext is { CurrentState: not null })
        {
            await _contextAccessor.CurrentContext.CurrentState
                .ExitAsync(_contextAccessor.CurrentContext, stoppingToken);
        }

        var previousState = _contextAccessor.CurrentContext.CurrentState;
        _contextAccessor.CurrentContext.CurrentState = newState;

        try
        {
            await newState.EnterAsync(_contextAccessor.CurrentContext, stoppingToken);
        }
        catch (Exception ex)
        {
            _contextAccessor.CurrentContext.CurrentState = previousState;
            throw new StateTransitionException($"Failed to enter state {typeof(T).Name}", ex);
        }
    }

    public async Task StartAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

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
                        await CurrentState.ExecuteAsync(_contextAccessor.CurrentContext, stoppingToken);
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was canceled cause {@Reason}",
                        _contextAccessor.CurrentContext.LoopName, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was failed cause {@Reason}",
                        _contextAccessor.CurrentContext.LoopName, ex.Message);
                    throw;
                }
            }
        }, _executionCts.Token);

        await _executionTask;
    }

    public async Task StopAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _executionCts?.CancelAsync();

        if (CurrentState is not null)
        {
            await CurrentState.ExitAsync(_contextAccessor.CurrentContext);
        }

        if (_executionTask is not null)
        {
            await Task.WhenAny(_executionTask);
        }
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

    private IGameLoopState GetState<T>() where T : IGameLoopState
    {
        if (!_interceptors.Any())
        {
            return _stateProvider.GetState<T>();
        }

        var innerState = _stateProvider.GetState<T>();
        return new InterceptingGameLoopState(innerState, new CompositeInterceptor(_interceptors));
    }
}
