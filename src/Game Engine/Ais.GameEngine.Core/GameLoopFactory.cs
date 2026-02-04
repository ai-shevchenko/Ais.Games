using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Core;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly IConfiguration _configuration;
    private readonly IKeyedModuleLoader _moduleLoader;
    private readonly IServiceCollection _rootServices;

    public GameLoopFactory(
        IServiceCollection rootServices,
        IConfiguration configuration,
        IKeyedModuleLoader moduleLoader)
    {
        _rootServices = rootServices;
        _configuration = configuration;
        _moduleLoader = moduleLoader;
    }

    public IGameLoop Create(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        var loopServices = new ServiceCollection();
        foreach (var service in _rootServices)
        {
            loopServices.Add(service);
        }

        foreach (var module in _moduleLoader.GetLoadedModules(name))
        {
            module.ConfigureGameServices(loopServices, _configuration);
        }

        var settings = new GameLoopBuilderSettings(loopServices);
        configure?.Invoke(settings);

        loopServices.AddSingleton<IGameLoop, GameLoop>();
        var provider = loopServices.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var loop = scope.ServiceProvider.GetRequiredService<IGameLoop>();

        var accessor = scope.ServiceProvider.GetRequiredService<IGameLoopContextAccessor>();
        accessor.CurrentContext = new GameLoopContext { LoopName = name };

        return loop;
    }
}
