using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, GameLoopScope> _cachedScopes = [];
    private readonly IGameLoopFactory _factory;
    private bool _disposed;

    public GameEngine(IGameLoopFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<IGameLoop> GameLoops => _cachedScopes.Values
        .Select(x => x.GameLoop)
        .ToArray();

    public IGameLoop GetGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedScopes.TryGetValue(name, out var scope)
            ? scope.GameLoop
            : throw new KeyNotFoundException(name);
    }

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedScopes.ContainsKey(name))
        {
            throw new InvalidOperationException($"Loop already exists with name {name}");
        }

        var scope = _factory.Create(name, configure);
        _cachedScopes.TryAdd(name, scope);

        return scope.GameLoop;
    }

    public bool HasGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _cachedScopes.ContainsKey(name);
    }

    public void Start(CancellationToken stoppingToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var scope in _cachedScopes.Values)
        {
            scope.GameLoop.Start(stoppingToken);
        }
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        DisposeGameLoops(stopOnly: true);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        DisposeGameLoops(stopOnly: false);
    }

    private void DisposeGameLoops(bool stopOnly)
    {
        var exceptions = new List<Exception>();

        foreach (var scope in _cachedScopes.Values)
        {
            try
            {
                if (stopOnly && scope.GameLoop.IsRunning)
                {
                    scope.GameLoop.Stop();
                }

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
