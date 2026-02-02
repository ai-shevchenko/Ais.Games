using System.Reflection;

using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly IServiceProvider _gameServices;
    private readonly IKeyedModuleLoader _moduleLoader;
    
    public GameLoopFactory(IServiceProvider gameServices, IKeyedModuleLoader moduleLoader)
    {
        _gameServices = gameServices;
        _moduleLoader = moduleLoader;
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
        
        // TODO: Вынести ключ в константу
        LoadModules("Default", settings);
        LoadModules(name, settings);

        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }

    public IGameLoop CreateGameLoop(string name, GameLoopBuilderSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var loopScope = _gameServices.CreateScope();

        var contextAccessor = loopScope.ServiceProvider
            .GetRequiredService<IGameLoopContextAccessor>();

        ConfigureContext(name, settings, contextAccessor);

        // TODO: Вынести ключ в константу
        LoadModules("Default", settings);
        LoadModules(name, settings);
        
        return ActivatorUtilities.CreateInstance<GameLoop>(loopScope.ServiceProvider, loopScope);
    }
    
    private void LoadModules(string name, GameLoopBuilderSettings settings)
    {
        foreach (var module in _moduleLoader.GetLoadedModules(name))
        {
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