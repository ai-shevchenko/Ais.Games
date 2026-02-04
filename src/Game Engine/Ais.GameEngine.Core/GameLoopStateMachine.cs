using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Interceptors;
using Ais.GameEngine.Core.States;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopStateMachine : IGameLoopStateMachine
{
    private readonly Lazy<GameLoopContext> _context;
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
        _context = new Lazy<GameLoopContext>(() =>
            contextAccessor.CurrentContext ?? throw new InvalidOperationException("Context was not initialized"));
        _logger = logger;
        _interceptors = interceptors;
    }

    public IGameLoopState? CurrentState => _context.Value.CurrentState;

    public async Task ChangeStateAsync<T>(CancellationToken stoppingToken = default)
        where T : IGameLoopState
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var newState = GetState<T>();

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

    private IGameLoopState GetState<T>() where T : IGameLoopState
    {
        if (_interceptors.Any())
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
