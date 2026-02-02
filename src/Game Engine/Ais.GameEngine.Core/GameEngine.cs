using Ais.GameEngine.Core.Abstractions;

using System.Collections.Concurrent;

using Ais.GameEngine.Modules.Abstractions;

namespace Ais.GameEngine.Core;

public sealed class GameEngine : IGameEngine, IDisposable
{
    private bool _disposed;
    
    private readonly ConcurrentDictionary<string, IGameLoop> _cachedLoops = [];
    private readonly IGameLoopFactory _gameLoopFactory;

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

    public IGameLoop GetOrCreateGameLoop(string name, Action<GameLoopBuilderSettings> configure)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedLoops.TryGetValue(name, out var item))
        {
            return item;
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
        if (_disposed) return;

        _disposed = true;
        
        Stop();

        foreach (var loop in _cachedLoops.Values)
        {
            loop.Dispose();
        }
    }
}
