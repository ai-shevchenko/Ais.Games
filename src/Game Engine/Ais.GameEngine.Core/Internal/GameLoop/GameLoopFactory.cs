using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;
using Ais.GameEngine.StateMachine.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal sealed class GameLoopFactory : IGameLoopFactory
{
    private readonly IConfiguration _configuration;
    private readonly IModuleLoader _moduleLoader;
    private readonly ILogger<GameLoopFactory> _logger;
    private readonly DependencyInjection.Abstractions.IServiceScopeFactory _serviceScopeFactory;

    public GameLoopFactory(
        IConfiguration configuration,
        IModuleLoader moduleLoader,
        ILogger<GameLoopFactory> logger,
        DependencyInjection.Abstractions.IServiceScopeFactory serviceScopeFactory)
    {
        _configuration = configuration;
        _moduleLoader = moduleLoader;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public GameLoopScope Create(string name, Action<GameLoopBuilderSettings>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _logger.LogInformation("Creating game loop '{LoopName}'", name);

        var scope = _serviceScopeFactory.CreateScope(name, builder =>
        {
            builder.ConfigureServices = (services) =>
            {
                foreach (var module in _moduleLoader.GetLoadedModules(name))
                {
                    module.ConfigureGameServices(services, _configuration);
                }

                var settings = new GameLoopBuilderSettings(services);
                configure?.Invoke(settings);

                services.AddSingleton<GameLoop>();
            };
        });

        var accessor = scope.Resolve<IGameContextAccessor>();
        accessor.CurrentContext = new GameContext { LoopName = name };

        var loop = scope.Resolve<GameLoop>();

        _logger.LogInformation("Game loop '{LoopName}' created successfully", name);

        return new GameLoopScope(name, loop, scope);
    }
}
