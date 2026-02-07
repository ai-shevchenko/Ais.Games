using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Core.Extensions;
using Ais.GameEngine.Core.Internal.StateMachine.States;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameLoop : IGameLoop
{
    private readonly ILogger<GameLoop> _logger;
    private readonly IGameStateMachine _stateMachine;
    private readonly Lock _sync = new();

    private bool _disposed;
    private CancellationTokenSource? _gameLoopCts;
    private Task? _gameLoopTask;

    public GameLoop(IGameStateMachine stateMachine, ILogger<GameLoop> logger)
    {
        _stateMachine = stateMachine;
        _logger = logger;
    }

    public bool IsPaused { get; private set; }
    public bool IsRunning { get; private set; }

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
            IsPaused = false;
            IsRunning = true;

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
            if (IsPaused || _gameLoopTask is { IsCompleted: true } or null)
            {
                return;
            }

            IsPaused = true;
            _ = _stateMachine.Pause(_gameLoopCts!.Token);
        }
    }

    public void Resume()
    {
        ObjectDisposedException.ThrowIf(_disposed, true);

        lock (_sync)
        {
            if (!IsPaused || _gameLoopTask == null || _gameLoopTask.IsCompleted)
            {
                return;
            }

            IsPaused = false;

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

            IsRunning = false;

            _gameLoopCts?.Cancel();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (IsRunning)
        {
            Stop();
        }

        _disposed = true;

        _stateMachine.Dispose();
        _gameLoopCts?.Dispose();
    }
}
