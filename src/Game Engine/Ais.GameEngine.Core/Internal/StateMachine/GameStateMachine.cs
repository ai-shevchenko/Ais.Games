using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Internal.StateMachine;

internal sealed class GameStateMachine : IGameStateMachine
{
    private readonly IGameContextAccessor _contextAccessor;
    private readonly ILogger<GameStateMachine> _logger;
    private readonly IGameStateExecutor _stateExecutor;
    private readonly IGameStateProvider _stateProvider;

    private bool _disposed;
    private CancellationTokenSource? _executionCts;
    private Task? _executionTask;
    private bool _isRunning;

    public GameStateMachine(
        IGameStateProvider stateProvider,
        IGameContextAccessor contextAccessor,
        ILogger<GameStateMachine> logger,
        IGameStateExecutor stateExecutor)
    {
        _stateProvider = stateProvider;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _stateExecutor = stateExecutor;
    }

    public IGameState? CurrentState => _contextAccessor.CurrentContext?.CurrentState;

    public async Task ChangeStateAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(_contextAccessor.CurrentContext);

        var newState = _stateProvider.GetState<T>();

        if (_contextAccessor.CurrentContext is { CurrentState: not null })
        {
            await _stateExecutor.ExitAsync(_contextAccessor.CurrentContext.CurrentState, stoppingToken);
        }

        var previousState = _contextAccessor.CurrentContext.CurrentState;
        _contextAccessor.CurrentContext.CurrentState = newState;

        try
        {
            await _stateExecutor.EnterAsync(newState, stoppingToken);
        }
        catch (Exception ex)
        {
            _contextAccessor.CurrentContext.CurrentState = previousState;
            throw new StateTransitionException($"Failed to enter state {typeof(T).Name}", ex);
        }
    }

    public async Task StartAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameState
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
        var execToken = _executionCts.Token;
        _executionTask = Task.Run(async () =>
        {
            while (_isRunning && !execToken.IsCancellationRequested)
            {
                try
                {
                    if (CurrentState is not null)
                    {
                        await _stateExecutor.ExecuteAsync(CurrentState, execToken);
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was canceled cause {@Reason}",
                        _contextAccessor.CurrentContext.LoopName, ex.Message);

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was failed cause {@Reason}",
                        _contextAccessor.CurrentContext.LoopName, ex.Message);

                    break;
                }
            }
        }, execToken);
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

        if (CurrentState is not null)
        {
            try
            {
                await _stateExecutor.ExitAsync(CurrentState, _executionCts!.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        if (_executionCts is not null)
        {
            await _executionCts.CancelAsync();
        }

        if (_executionTask != null)
        {
            try
            {
                await _executionTask;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopAsync()
            .GetAwaiter()
            .GetResult();

        _disposed = true;
    }
}
