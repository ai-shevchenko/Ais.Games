using Ais.GameEngine.Hooks.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.HooksSystem;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHooksSystem(this IServiceCollection services)
    {
        services.AddScoped<IHooksProvider, HooksProvider>();
        return services;
    }
}
