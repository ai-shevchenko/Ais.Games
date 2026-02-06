using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly IConfiguration _configuration;
    private readonly IKeyedModuleLoader _moduleLoader;
    private readonly IContainer _rootContainer;

    public GameLoopFactory(
        IContainer container,
        IConfiguration configuration,
        IKeyedModuleLoader moduleLoader)
    {
        _rootContainer = container;
        _configuration = configuration;
        _moduleLoader = moduleLoader;
    }

    public GameLoopScope Create(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        var loopServices = new ServiceCollection();

        foreach (var module in _moduleLoader.GetLoadedModules(name))
        {
            module.ConfigureGameServices(loopServices, _configuration);
        }

        var settings = new GameLoopBuilderSettings(loopServices);
        configure?.Invoke(settings);

        var scope = _rootContainer.BeginLifetimeScope(name, builder =>
        {
            builder.Populate(loopServices);

            builder.RegisterType<GameLoop>()
                .AsSelf()
                .InstancePerLifetimeScope();
        });

        var accessor = scope.Resolve<IGameContextAccessor>();
        accessor.CurrentContext = new GameContext { LoopName = name };

        var loop = scope.Resolve<GameLoop>();

        return new GameLoopScope(name, loop, scope);
    }
}
