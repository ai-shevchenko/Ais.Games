using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ais.GameEngine.Extensions.Ecs;

public static class EcsExtensions
{
    public static IEcsWorldBuilder AddEcs(this IServiceCollection services)
    {
        var builder = new EcsWorldBuilder(services);
        services.TryAddSingleton<IEcsWorldBuilder>(builder);
        return builder;
    }
}
