
using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateMachine : IGameLoopStateMachine, IDisposable
{
    private readonly IGameLoopStateFactory _stateFactory;
    private readonly Lazy<GameLoopContext> _context;
    private readonly ConcurrentDictionary<Type, IGameLoopState> _cachedStates = [];
    private readonly ILogger<GameLoopStateMachine> _logger;

    private Task? _executionTask;
    private CancellationTokenSource? _executionCts;
    private bool _isRunning;
    private bool _disposed;

    public IGameLoopState? CurrentState => _context.Value.CurrentState;

    public GameLoopStateMachine(
        IGameLoopStateFactory stateFactory, 
        IGameLoopContextAccessor contextAccessor, 
        ILogger<GameLoopStateMachine> logger)
    {
        _stateFactory = stateFactory;
        _context = new Lazy<GameLoopContext>(() => contextAccessor.CurrentContext ?? throw new InvalidOperationException());
        _logger = logger;
    }

    public void RegisterState<T>()
        where T : IGameLoopState
    {
        if (!_cachedStates.TryGetValue(typeof(T), out var state))
        {
            state = _stateFactory.CreateState<T>();
            _cachedStates.TryAdd(typeof(T), state);
        }
    }

    public async Task ChangeStateAsync<T>(CancellationToken stoppingToken = default) 
        where T : IGameLoopState
    {
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
                    _logger.LogError(ex, "State machine for {@GameLoop} was canceled cause {@Reason}", _context.Value.LoopName, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "State machine for {@GameLoop} was failed cause {@Reason}", _context.Value.LoopName, ex.Message);
                    throw;
                }
            }
        }, _executionCts.Token);

        await _executionTask;
    }

    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;

        _executionCts!.Cancel();
        await _executionTask!.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                _logger.LogInformation("Execution of state machine was canceled!");
            }
        });

        if (CurrentState is not null)
        {
            await CurrentState.ExitAsync(_context.Value);
        }
    }

    public void Dispose()
    {
        _ = StopAsync();

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