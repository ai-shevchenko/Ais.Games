using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Extensions;
using Ais.GameEngine.Core.States;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoop : IGameLoop, IDisposable
{    
    private readonly Lock _sync = new();
    private readonly IGameLoopStateMachine _stateMachine;
    private readonly ILogger<GameLoop> _logger;
    private readonly IDisposable _innerScope;

    private Task? _gameLoopTask;
    private CancellationTokenSource? _gameLoopCts;

    private bool _disposed;
    private bool _isPaused;

    public GameLoop(IGameLoopStateMachine stateMachine, ILogger<GameLoop> logger, IDisposable innerScope)
    {
        _stateMachine = stateMachine;
        _innerScope = innerScope;
        _logger = logger;
    }

    public void Start(CancellationToken stoppingToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, true);
        
        lock (_sync)
        {
            if (_gameLoopTask is { IsCompleted: true })
            {
                throw new InvalidOperationException("Game loop is already running");
            }

            _gameLoopCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            _isPaused = false;

            _gameLoopTask = Task.Run(async () =>
            {
                try
                {
                    await _stateMachine.StartAsync<InitializeState>(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Game loop starting was fault");
                }
            }, _gameLoopCts.Token);
        }
    }

    public void Pause()
    {
        ObjectDisposedException.ThrowIf(_disposed, true);
        
        lock (_sync)
        {
            if (_isPaused || _gameLoopTask is { IsCompleted: true } or null)
            {
                return;
            }

            _isPaused = true;
            _ = _stateMachine.Pause(_gameLoopCts!.Token);
        }
    }

    public void Resume()
    {
        ObjectDisposedException.ThrowIf(_disposed, true);
        
        lock (_sync)
        {
            if (!_isPaused || _gameLoopTask == null || _gameLoopTask.IsCompleted)
                return;

            _isPaused = false;

            _ = _stateMachine.Run(_gameLoopCts!.Token);
        }
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, true);
        
        lock (_sync)
        {
            if (_gameLoopTask is { IsCompleted: true } or null)
            {
                return;
            }

            try
            {
                _stateMachine.Stop().Wait();
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex.InnerException, "Game loop stopping was fault");
            }

            _gameLoopCts?.Cancel();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        Stop();

        _disposed = true;
        
        _gameLoopCts?.Dispose();
        _gameLoopTask?.Dispose();

        _stateMachine.Dispose();
        _innerScope.Dispose();
    }
}