using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Extensions;
using Ais.GameEngine.Core.States;
using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core;

internal sealed class GameLoop : IGameLoop
{
    private readonly ILogger<GameLoop> _logger;
    private readonly IGameLoopStateMachine _stateMachine;

    private readonly Lock _sync = new();
    private bool _disposed;
    private CancellationTokenSource? _gameLoopCts;
    private Task? _gameLoopTask;
    private bool _isPaused;

    public GameLoop(IGameLoopStateMachine stateMachine, ILogger<GameLoop> logger)
    {
        _stateMachine = stateMachine;
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
            {
                return;
            }

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
                _stateMachine.Stop(_gameLoopCts!.Token).Wait();
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex.InnerException, "Game loop stopping was fault");
            }

            _gameLoopCts?.Cancel();
            if (_gameLoopTask is not null)
            {
                Task.WhenAny(_gameLoopTask).Wait();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();

        _disposed = true;

        _stateMachine.Dispose();
        _gameLoopCts?.Dispose();
    }
}
