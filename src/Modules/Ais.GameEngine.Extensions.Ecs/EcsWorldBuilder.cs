using Ais.ECS;
using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ais.GameEngine.Extensions.Ecs;

public sealed class EcsWorldBuilder : IEcsWorldBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<IServiceProvider, IWorld>> _setup = [];

    public EcsWorldBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IServiceCollection Services => _services;

    public IEcsWorldBuilder WithSystem<T>()
        where T : class, ISystem
    {
        _services.AddScoped<T>();
        _services.AddScoped<ISystem, T>();
        return this;
    }

    public IEcsWorldBuilder WithWorldSetup(Action<IServiceProvider, IWorld> configure)
    {
        _setup.Add(configure);
        return this;
    }

    public IWorld Build(IServiceProvider gameServices)
    {
        var settings = gameServices.GetRequiredService<IOptions<EcsSettings>>().Value;
        var world = new World(settings);

        foreach (var system in gameServices.GetServices<ISystem>())
        {
            var context = new SystemContext(world);
            system.Initialize(context);
            world.AddSystem(system);
        }

        foreach (var configure in _setup)
        {
            configure(gameServices, world);
        }

        return world;
    }
}
