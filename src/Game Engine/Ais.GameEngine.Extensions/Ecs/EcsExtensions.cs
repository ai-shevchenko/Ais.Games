using Ais.Commons.Modules;
using Ais.ECS.Abstractions.Worlds;
using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

public static class EcsExtensions
{
    public static IEcsWorldBuilder AddEcsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule<EcsModule>(configuration);
        
        var builder = new EcsWorldBuilder();
        services.AddSingleton<IEcsWorldBuilder>(builder);

        return builder;
    }
    
    public static GameLoopBuilderSettings InitializeEcsLoop(this GameLoopBuilderSettings gameLoopSettings,
        Action<IServiceProvider, IWorld> configure)
    {
        var world = gameLoopSettings.GameServices.GetRequiredService<IWorld>();
        configure(gameLoopSettings.GameServices, world);

        gameLoopSettings.Hooks.AddHook<EcsSystemHandlerHook>();
        gameLoopSettings.Hooks.AddHook<EcsSystemAsyncHandlerHook>();

        return gameLoopSettings;
    }
}