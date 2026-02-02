using Ais.ECS;
using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ais.GameEngine.Extensions.Ecs;

public sealed class EcsWorldBuilder : IEcsWorldBuilder
{
    private static readonly Lock _sync = new();
    private readonly List<Type> _systems = [];

    public static EcsWorldBuilder Instance
    {
        get
        {
            lock (_sync)
            {
                return field ??= new EcsWorldBuilder(); 
            }
        }
    }
    
    public IEcsWorldBuilder WithSystem<T>()
        where T : class, ISystem
    {
        _systems.Add(typeof(T));
        return this;
    }

    public IWorld Build(IServiceProvider gameServices)
    {
        var settings = gameServices.GetRequiredService<IOptions<EcsSettings>>().Value;
        var world = new World(settings);

        foreach (var systemType in _systems)
        {
            var system = (ISystem) ActivatorUtilities.CreateInstance(gameServices, systemType);
            world.AddSystem(system);
        }

        return world;
    }
}