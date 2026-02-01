using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.Commons.Modules;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModule<T>(this IServiceCollection services, IConfiguration configuration)
        where T : IModule, new()
    {
        var module = new T();
        module.Configure(services, configuration);
        return services;
    }
}