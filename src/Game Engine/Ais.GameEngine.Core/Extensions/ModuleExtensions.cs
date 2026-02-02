using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Modules.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Core.Extensions;

public static class ModuleExtensions
{
    public static GameEngineBuilder AddModule<T>(this GameEngineBuilder builder)
        where T : GameEngineModule, new()
    {
        var module = new T();
        builder.ConfigureGameServices((context, services) => module.ConfigureGameServices(services, context.Configuration));
        return builder;
    }
    
    public static IServiceCollection AddModule<T>(this IServiceCollection services, IConfiguration configuration)
        where T : GameEngineModule, new()
    {
        var module = new T();
        module.ConfigureGameServices(services, configuration);
        return services;
    }

    public static GameLoopBuilderSettings UseModule<T>(this GameLoopBuilderSettings settings)
        where T : GameEngineModule, new()
    {
        var module = new T();
        module.ConfigureGameLoop(settings);
        return settings;
    }
}