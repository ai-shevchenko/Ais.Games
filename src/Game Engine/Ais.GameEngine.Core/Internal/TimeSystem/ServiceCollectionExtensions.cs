using Ais.GameEngine.TimeSystem.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Internal.TimeSystem;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimeSystem(this IServiceCollection services)
    {
        services
            .AddSingleton<ITimerController, TimerController>()
            .AddTransient(sp => sp.GetRequiredService<ITimerController>().CreateChildTimer());

        return services;
    }
}
