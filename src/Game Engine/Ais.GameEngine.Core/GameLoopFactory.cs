using System.Reflection;

using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly GameLoopFactorySettings _settings;
    private readonly IServiceProvider _gameServices;

    public GameLoopFactory(IServiceProvider gameServices, GameLoopFactorySettings settings)
    {
        _gameServices = gameServices;
        _settings = settings;
    }

    public IGameLoop CreateGameLoop(string name, Action<GameLoopBuilderSettings> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var loopScope = _gameServices.CreateScope();

        var contextAccessor = loopScope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        var settings = ActivatorUtilities.CreateInstance<GameLoopBuilderSettings>(loopScope.ServiceProvider);
        ConfigureContext(name, settings, contextAccessor);

        configure(settings);

        ConfigureGameLoopModules(name, settings);

        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }

    public IGameLoop CreateGameLoop(string name, GameLoopBuilderSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var loopScope = _gameServices.CreateScope();

        var contextAccessor = loopScope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        ConfigureContext(name, settings, contextAccessor);

        ConfigureGameLoopModules(name, settings);
        
        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }

    private void ConfigureGameLoopModules(string name, GameLoopBuilderSettings settings)
    {
        if (_settings.AssemblyModules.TryGetValue(name, out var assemblies))
        {
            foreach (var assembly in assemblies)
            {
                ConfigureGameLoopFromAssembly(assembly, settings);
            }
        }

        if (_settings.DllModules.TryGetValue(name, out var dlls))
        {
            foreach (var dll in dlls)
            {
                ConfigureGameLoopFromAssembly(Assembly.LoadFrom(dll), settings);
            }
        }
    }
    
    private static void ConfigureGameLoopFromAssembly(Assembly assembly, GameLoopBuilderSettings settings)
    {
        var typeEnumerator = assembly.GetTypes()
            .Where(t => typeof(GameEngineModule).IsAssignableFrom(t));

        foreach (var type in typeEnumerator)
        {
            var module =  (GameEngineModule)Activator.CreateInstance(type)!;
            module.ConfigureGameLoop(settings);
        }
    }
    
    private static void ConfigureContext(string name, GameLoopBuilderSettings settings, IGameLoopContextAccessor contextAccessor)
    {
        var context = new GameLoopContext
        {
            Hooks = settings.Hooks,
            LoopName = name
        };

        contextAccessor.CurrentContext = context;
    }
}