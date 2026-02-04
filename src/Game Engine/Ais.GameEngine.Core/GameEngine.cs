using System.Collections.Concurrent;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Core;

internal sealed class GameEngine : IGameEngine
{
    private readonly ConcurrentDictionary<string, GameLoopScope> _cachedScopes = [];
    private readonly IConfiguration _configuration;
    private readonly IKeyedModuleLoader _moduleLoader;
    private readonly IServiceCollection _services;
    private bool _disposed;

    public GameEngine(
        IServiceCollection services,
        IKeyedModuleLoader moduleLoader,
        IConfiguration configuration)
    {
        _services = services;
        _moduleLoader = moduleLoader;
        _configuration = configuration;
    }

    public IReadOnlyList<IGameLoop> GameLoops => _cachedScopes.Values
        .Select(x => x.Loop)
        .ToList()
        .AsReadOnly();

    public IGameLoop GetGameLoop(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _cachedScopes.TryGetValue(name, out var item)
            ? item.Loop
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

        var loopServices = new ServiceCollection { _services };
        foreach (var module in _moduleLoader.GetLoadedModules(name))
        {
            module.ConfigureGameServices(loopServices, _configuration);
        }

        var settings = new GameLoopBuilderSettings(loopServices);

        configure?.Invoke(settings);

        loopServices.AddSingleton<IGameLoop, GameLoop>();
        var provider = loopServices.BuildServiceProvider();

        var scope = provider.CreateScope();

        var loop = scope.ServiceProvider.GetRequiredService<IGameLoop>();
        var loopScope = new GameLoopScope(name, loop, scope);

        var accessor = scope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        accessor.CurrentContext = new GameLoopContext { LoopName = name };

        _cachedScopes.TryAdd(name, loopScope);

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
            item.Loop.Start(stoppingToken);
        }
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var item in _cachedScopes.Values)
        {
            item.Loop.Stop();
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

    private sealed class GameLoopScope : IDisposable
    {
        public GameLoopScope(string name, IGameLoop loop, IServiceScope scope)
        {
            Name = name;
            Loop = loop;
            Scope = scope;
        }

        public IGameLoop Loop { get; }
        public IServiceScope Scope { get; }
        public string Name { get; }

        public void Dispose()
        {
            Loop.Dispose();
            Scope.Dispose();
        }
    }
}
