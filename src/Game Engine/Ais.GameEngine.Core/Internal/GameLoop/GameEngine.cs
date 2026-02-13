using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, GameLoopScope> _cachedScopes = [];
    private readonly IGameLoopFactory _factory;
    private readonly SemaphoreSlim StateLock = new(1, 1);
    private bool _disposed;

    public GameEngine(IGameLoopFactory factory)
    {
        _factory = factory;
        State = EngineState.Idle;
    }

    public IReadOnlyList<IGameLoop> GameLoops => [.. _cachedScopes.Values.Select(x => x.GameLoop)];

    public EngineState State { get; private set; } = EngineState.NotInitialized;

    public IGameLoop GetGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedScopes.TryGetValue(name, out var scope)
            ? scope.GameLoop
            : throw new KeyNotFoundException(name);
    }

    public bool TryGetGameLoop(string name, out IGameLoop gameLoop)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedScopes.TryGetValue(name, out var scope))
        {
            gameLoop = scope.GameLoop;
            return true;
        }

        gameLoop = null!;
        return false;
    }

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedScopes.ContainsKey(name))
        {
            throw new InvalidOperationException($"Loop already exists with name '{name}'");
        }

        var scope = _factory.Create(name, configure);
        _cachedScopes.TryAdd(name, scope);

        return scope.GameLoop;
    }

    public bool HasGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedScopes.ContainsKey(name);
    }

    public async Task<bool> RemoveGameLoopAsync(
        string name,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        timeout ??= TimeSpan.FromSeconds(5);

        if (!_cachedScopes.TryRemove(name, out var scope))
        {
            return false;
        }

        try
        {
            if (scope.GameLoop.IsRunning)
            {
                await scope.GameLoop.StopAsync(timeout, cancellationToken).ConfigureAwait(false);
            }

            scope.Dispose();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task StartAsync(CancellationToken stoppingToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await StateLock.WaitAsync(stoppingToken).ConfigureAwait(false);
        try
        {
            if (State != EngineState.Idle)
            {
                throw new InvalidOperationException(
                    $"Cannot start game engine in state '{State}'. Only '{EngineState.Idle}' state is allowed.");
            }

            State = EngineState.Running;
        }
        finally
        {
            StateLock.Release();
        }

        var tasks = _cachedScopes.Values
            .Select(scope => scope.GameLoop.StartAsync(stoppingToken))
            .ToList();

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await StateLock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
            try
            {
                State = EngineState.Failed;
            }
            finally
            {
                StateLock.Release();
            }

            throw;
        }
    }

    public async Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        timeout ??= TimeSpan.FromSeconds(10);

        await StateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State == EngineState.Stopped)
            {
                return;
            }

            State = EngineState.Stopping;
        }
        finally
        {
            StateLock.Release();
        }

        await StopGameLoopsAsync(timeout ?? TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

        await StateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            State = EngineState.Stopped;
        }
        finally
        {
            StateLock.Release();
        }
    }

    public async Task PauseAllAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var tasks = _cachedScopes.Values
            .Where(scope => scope.GameLoop.IsRunning)
            .Select(scope => scope.GameLoop.PauseAsync(cancellationToken))
            .ToList();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task ResumeAllAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var tasks = _cachedScopes.Values
            .Where(scope => scope.GameLoop.IsPaused)
            .Select(scope => scope.GameLoop.ResumeAsync(cancellationToken))
            .ToList();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            StopAsync(TimeSpan.FromSeconds(10))
                .GetAwaiter()
                .GetResult();
        }
        catch
        {
        }

        DisposeGameLoops(false);

        _cachedScopes.Clear();
        StateLock.Dispose();
    }

    private async Task StopGameLoopsAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var scope in _cachedScopes.Values)
        {
            try
            {
                if (scope.GameLoop.IsRunning)
                {
                    await scope.GameLoop.StopAsync(timeout, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(
                "One or more errors occurred while stopping game loops.",
                exceptions);
        }
    }

    private void DisposeGameLoops(bool stopOnly)
    {
        var exceptions = new List<Exception>();

        foreach (var scope in _cachedScopes.Values)
        {
            try
            {
                if (!stopOnly)
                {
                    scope.Dispose();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(
                "One or more errors occurred while disposing game loops.",
                exceptions);
        }
    }
}
