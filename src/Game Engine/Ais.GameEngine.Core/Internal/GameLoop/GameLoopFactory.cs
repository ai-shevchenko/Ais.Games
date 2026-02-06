using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Core.Internal.GameLoop;

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

    public GameLoopScope Create(string name, Action<GameLoopBuilderSettings>? configure = null)
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

        var provider = loopServices.BuildServiceProvider();
        var scope = provider.CreateScope();

        var accessor = scope.ServiceProvider.GetRequiredService<IGameContextAccessor>();
        accessor.CurrentContext = new GameContext { LoopName = name };

        var loop = ActivatorUtilities.CreateInstance<GameLoop>(scope.ServiceProvider);

        return new GameLoopScope(name, loop, scope);
    }
}
