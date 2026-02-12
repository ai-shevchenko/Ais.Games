using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Extensions;
using Ais.GameEngine.Core.Internal.StateMachine.States;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Internal.GameLoop;

/// <summary>
/// Асинхронная реализация игрового цикла
/// </summary>
internal sealed class GameLoop : IGameLoop
{
    private const int DefaultStopTimeoutMs = 5000;

    private readonly ILogger<GameLoop> _logger;
    private readonly IGameStateMachine _stateMachine;
    private readonly IGameContextAccessor _contextAccessor;
    private readonly IGameLoopEventBus _eventBus;
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    private bool _disposed;
    private CancellationTokenSource? _gameLoopCts;
    private Task? _gameLoopTask;

    public GameLoop(
        IGameStateMachine stateMachine,
        ILogger<GameLoop> logger,
        IGameContextAccessor contextAccessor,
        IGameLoopEventBus eventBus)
    {
        _stateMachine = stateMachine;
        _logger = logger;
        _contextAccessor = contextAccessor;
        _eventBus = eventBus;
    }

    public string Name => _contextAccessor.CurrentContext?.LoopName ?? "Unknown";

    public GameLoopState State { get; private set; } = GameLoopState.Stopped;

    public bool IsRunning => State is GameLoopState.Running;

    public bool IsPaused => State is GameLoopState.Paused;

    public event EventHandler<GameLoopEventArgs>? Started;
    public event EventHandler<GameLoopEventArgs>? Stopped;
    public event EventHandler<GameLoopEventArgs>? Paused;
    public event EventHandler<GameLoopEventArgs>? Resumed;
    public event EventHandler<GameLoopErrorEventArgs>? ErrorOccurred;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State != GameLoopState.Stopped)
            {
                throw new InvalidOperationException(
                    $"Cannot start game loop in state '{State}'. Only '{GameLoopState.Stopped}' state is allowed.");
            }

            await ChangeStateAsync(GameLoopState.Initializing, cancellationToken);

            _gameLoopCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var token = _gameLoopCts.Token;

            _gameLoopTask = RunGameLoopAsync(token);
        }
        finally
        {
            _stateLock.Release();
        }
    }

    public async Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        timeout ??= TimeSpan.FromMilliseconds(DefaultStopTimeoutMs);

        await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State == GameLoopState.Stopped)
            {
                return;
            }

            if (State == GameLoopState.Initializing)
            {
                await ChangeStateAsync(GameLoopState.Stopping, cancellationToken)
                    .ConfigureAwait(false);

                await WaitForStateChangeAsync(GameLoopState.Running, timeout.Value, cancellationToken)
                    .ConfigureAwait(false);
            }

            await ChangeStateAsync(GameLoopState.Stopping, cancellationToken)
                .ConfigureAwait(false);

            if (_gameLoopCts != null && !_gameLoopCts.Token.IsCancellationRequested)
            {
                await _gameLoopCts.CancelAsync();
            }

            await AttemptStopAsync(timeout.Value)
                .ConfigureAwait(false);
        }
        finally
        {
            await ChangeStateAsync(GameLoopState.Stopped, CancellationToken.None)
                .ConfigureAwait(false);

            _stateLock.Release();
        }

        OnStopped();
    }

    public async Task PauseAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State != GameLoopState.Running)
            {
                throw new InvalidOperationException(
                    $"Cannot pause game loop in state '{State}'. Only '{GameLoopState.Running}' state is allowed.");
            }

            await ChangeStateAsync(GameLoopState.Paused, cancellationToken)
                .ConfigureAwait(false);

            if (_gameLoopCts != null)
            {
                await _stateMachine.Pause(_gameLoopCts.Token);
            }
        }
        finally
        {
            _stateLock.Release();
        }

        OnPaused();
    }

    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State != GameLoopState.Paused)
            {
                throw new InvalidOperationException(
                    $"Cannot resume game loop in state '{State}'. Only '{GameLoopState.Paused}' state is allowed.");
            }

            await ChangeStateAsync(GameLoopState.Running, cancellationToken)
                .ConfigureAwait(false);

            if (_gameLoopCts != null)
            {
                await _stateMachine.Run(_gameLoopCts.Token);
            }
        }
        finally
        {
            _stateLock.Release();
        }

        OnResumed();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (State is GameLoopState.Running or GameLoopState.Paused or GameLoopState.Initializing)
        {
            try
            {
                StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing game loop");
            }
        }

        _gameLoopTask?.GetAwaiter()
            .GetResult();

        _stateMachine.Dispose();
        _gameLoopCts?.Dispose();
        _stateLock.Dispose();
    }

    private async Task RunGameLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _stateMachine.StartAsync<InitializeState>(cancellationToken).ConfigureAwait(false);
            await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (State == GameLoopState.Initializing)
                {
                    await ChangeStateAsync(GameLoopState.Running, cancellationToken);
                }
            }
            finally
            {
                _stateLock.Release();
            }

            OnStarted();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Game loop '{LoopName}' was cancelled", Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Game loop '{LoopName}' encountered an error", Name);

            await _stateLock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
            try
            {
                await ChangeStateAsync(GameLoopState.Failed, CancellationToken.None);
            }
            finally
            {
                _stateLock.Release();
            }

            OnErrorOccurred(ex, $"Game loop failed with error: {ex.Message}");
        }
    }

    private async Task AttemptStopAsync(TimeSpan timeout)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            await _stateMachine.StopAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Game loop '{LoopName}' did not stop within {TimeoutMs}ms, forcing shutdown",
                Name,
                timeout.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping game loop '{LoopName}'", Name);
            OnErrorOccurred(ex, $"Error stopping game loop: {ex.Message}");
        }
    }

    private async Task ChangeStateAsync(GameLoopState state, CancellationToken cancellationToken)
    {
        State = state;
        await _eventBus.PublishAsync(new GameLoopStateChangedEvent
        {
            SourceLoopName = Name,
            State = state
        }, cancellationToken).ConfigureAwait(false);
    }

    private async Task WaitForStateChangeAsync(
        GameLoopState expectedState,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        var elapsed = 0;
        while (State != expectedState && elapsed < (int)timeout.TotalMilliseconds)
        {
            try
            {
                await Task.Delay(10, linkedCts.Token).ConfigureAwait(false);
                elapsed += 10;
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException(
                    $"Timeout waiting for game loop '{Name}' to reach state '{expectedState}'");
            }
        }

        if (State != expectedState)
        {
            throw new TimeoutException(
                $"Game loop '{Name}' did not reach state '{expectedState}' within {timeout.TotalMilliseconds}ms");
        }
    }

    private void OnStarted()
    {
        var args = new GameLoopEventArgs { LoopName = Name };
        Started?.Invoke(this, args);
    }

    private void OnStopped()
    {
        var args = new GameLoopEventArgs { LoopName = Name };
        Stopped?.Invoke(this, args);
    }

    private void OnPaused()
    {
        var args = new GameLoopEventArgs { LoopName = Name };
        Paused?.Invoke(this, args);
    }

    private void OnResumed()
    {
        var args = new GameLoopEventArgs { LoopName = Name };
        Resumed?.Invoke(this, args);
    }

    private void OnErrorOccurred(Exception ex, string message)
    {
        var args = new GameLoopErrorEventArgs
        {
            LoopName = Name,
            Exception = ex,
            ErrorMessage = message
        };
        ErrorOccurred?.Invoke(this, args);
    }
}
