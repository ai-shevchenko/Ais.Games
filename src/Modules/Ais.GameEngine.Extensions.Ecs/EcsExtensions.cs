using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

public static class EcsExtensions
{
    public static IEcsWorldBuilder AddEcs(this IServiceCollection services)
    {
        return EcsWorldBuilder.Instance;
    }
}
