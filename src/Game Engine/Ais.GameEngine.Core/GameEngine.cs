using System.Collections.Concurrent;

using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Core;

internal sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, IGameLoop> _cachedScopes = [];
    private readonly IConfiguration _configuration;
    private readonly IGameLoopFactory _factory;
    private bool _disposed;

    public GameEngine(IGameLoopFactory factory, IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
    }

    public IReadOnlyList<IGameLoop> GameLoops => _cachedScopes.Values
        .ToArray();

    public IGameLoop GetGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedScopes.TryGetValue(name, out var item)
            ? item
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

        var loop = _factory.Create(name, configure);
        _cachedScopes.TryAdd(name, loop);

        return loop;
    }

    public bool HasGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _cachedScopes.ContainsKey(name);
    }

    public void Start(CancellationToken stoppingToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var item in _cachedScopes.Values)
        {
            item.Start(stoppingToken);
        }
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var item in _cachedScopes.Values)
        {
            item.Stop();
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

        foreach (var item in _cachedScopes.Values)
        {
            item.Dispose();
        }
    }
}
