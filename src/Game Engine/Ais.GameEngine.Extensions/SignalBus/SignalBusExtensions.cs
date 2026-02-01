using Ais.Commons.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.SignalBus;

public static class SignalBusExtensions
{
    public static IServiceCollection AddSignalBus(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddModule<SignalBusModule>(configuration);
    }
}