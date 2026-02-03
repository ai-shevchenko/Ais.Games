using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.States;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateMachine : IGameLoopStateMachine
{
    private readonly ConcurrentDictionary<Type, IGameLoopState> _cachedStates = [];
    private readonly Lazy<GameLoopContext> _context;
    private readonly ILogger<GameLoopStateMachine> _logger;

    private readonly IGameLoopStateFactory _stateFactory;
    private bool _disposed;
    private CancellationTokenSource? _executionCts;
    private Task? _executionTask;
    private bool _isRunning;

    public GameLoopStateMachine(
        IGameLoopStateFactory stateFactory,
        IGameLoopContextAccessor contextAccessor,
        ILogger<GameLoopStateMachine> logger)
    {
        _stateFactory = stateFactory;
        _context = new Lazy<GameLoopContext>(() =>
            contextAccessor.CurrentContext ?? throw new InvalidOperationException());
        _logger = logger;
    }

    public IGameLoopState? CurrentState => _context.Value.CurrentState;

    public void RegisterState<T>()
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_cachedStates.TryGetValue(typeof(T), out var state))
        {
            state = _stateFactory.CreateState<T>();
            _cachedStates.TryAdd(typeof(T), state);
        }
    }

    public async Task ChangeStateAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_cachedStates.TryGetValue(typeof(T), out var newState))
        {
            RegisterState<T>();
            newState = _cachedStates[typeof(T)];
        }

        if (_context.Value.CurrentState is not null)
        {
            await _context.Value.CurrentState.ExitAsync(_context.Value, stoppingToken);
        }

        var previousState = _context.Value.CurrentState;
        _context.Value.CurrentState = newState;

        try
        {
            await newState.EnterAsync(_context.Value, stoppingToken);
        }
        catch (Exception ex)
        {
            _context.Value.CurrentState = previousState;
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
                        await CurrentState.ExecuteAsync(_context.Value, stoppingToken);
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was canceled cause {@Reason}",
                        _context.Value.LoopName, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was failed cause {@Reason}",
                        _context.Value.LoopName, ex.Message);
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
            await CurrentState.ExitAsync(_context.Value);
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

        foreach (var state in _cachedStates.Values)
        {
            if (state is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _cachedStates.Clear();
    }
}
