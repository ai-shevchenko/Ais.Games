using Ais.ECS.Abstractions.Worlds;
using Ais.GameEngine.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

public static class EcsExtensions
{
    public static IEcsWorldBuilder AddEcs(this IServiceCollection services)
    {
        return EcsWorldBuilder.Instance;
    }

    public static GameLoopBuilderSettings InitializeEcsLoop(this GameLoopBuilderSettings gameLoopSettings,
        Action<IServiceProvider, IWorld> configure)
    {
        var world = gameLoopSettings.GameServices.GetRequiredService<IWorld>();
        configure(gameLoopSettings.GameServices, world);

        return gameLoopSettings;
    }
}
