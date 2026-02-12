using Ais.GameEngine.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.GameLoop;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameLoopServices(this IServiceCollection services)
    {
        services.AddSingleton<IGameLoopEventBus, GameLoopEventBus>();
        return services;
    }
}
