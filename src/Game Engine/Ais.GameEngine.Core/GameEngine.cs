using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core;

public sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, IGameLoop> _cachedLoops = [];
    private readonly IGameLoopFactory _gameLoopFactory;
    private bool _disposed;

    public GameEngine(IGameLoopFactory factory)
    {
        _gameLoopFactory = factory;
    }

    public IReadOnlyList<IGameLoop> GameLoops => _cachedLoops.Values
        .ToList()
        .AsReadOnly();

    public IGameLoop GetGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedLoops.TryGetValue(name, out var item)
            ? item
            : throw new KeyNotFoundException(name);
    }

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings> configure)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedLoops.TryGetValue(name, out _))
        {
            throw new InvalidOperationException($"Loop already exists with name {name}");
        }

        var gameLoop = _gameLoopFactory.CreateGameLoop(name, configure);
        _cachedLoops.TryAdd(name, gameLoop);

        return gameLoop;
    }

    public bool HasGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _cachedLoops.ContainsKey(name);
    }

    public void Start(CancellationToken stoppingToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var loop in _cachedLoops.Values)
        {
            loop.Start(stoppingToken);
        }
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var loop in _cachedLoops.Values)
        {
            loop.Stop();
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

        foreach (var loop in _cachedLoops.Values)
        {
            loop.Dispose();
        }
    }
}
