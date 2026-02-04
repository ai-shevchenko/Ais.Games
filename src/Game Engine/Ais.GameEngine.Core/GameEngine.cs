using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Core;

internal sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, GameLoopScope> _cachedLoops = [];
    private readonly IServiceProvider _rootProvider;

    private bool _disposed;

    public GameEngine(IServiceProvider rootProvider)
    {
        _rootProvider = rootProvider;
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

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_cachedLoops.ContainsKey(name))
        {
            throw new InvalidOperationException($"Loop already exists with name {name}");
        }

        var settings = new GameLoopBuilderSettings(loopServices);

        foreach (var serviceDescriptor in _services)
        {
            loopServices.Add(serviceDescriptor);
        }

        configure?.Invoke(settings);

        loopServices.AddKeyedSingleton<IGameLoop, GameLoop>(name);
        var gameServices = loopServices.BuildServiceProvider();
        var gameLoop = gameServices.GetRequiredService<IGameLoop>();

        var accessor = gameServices.GetRequiredService<IGameLoopContextAccessor>();
        accessor.CurrentContext = new GameLoopContext
        {
            LoopName = name,
            Hooks = gameServices.GetRequiredService<IHooksProvider>(),
            StatesProvider = gameServices.GetRequiredService<IGameLoopStateProvider>()
        };

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

    private class GameLoopScope : IDisposable
    {
        public IGameLoop Loop { get; }
        public IServiceScope Scope { get; }
        public string Name { get; }

        public GameLoopScope(string name, IGameLoop loop, IServiceScope scope)
        {
            Name = name;
            Loop = loop;
            Scope = scope;
        }

        public void Dispose()
        {
            Loop.Dispose();
            Scope.Dispose();
        }
    }
}
