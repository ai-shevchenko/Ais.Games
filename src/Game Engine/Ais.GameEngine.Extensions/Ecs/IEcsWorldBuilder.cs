using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;

namespace Ais.GameEngine.Extensions.Ecs;

public interface IEcsWorldBuilder
{
    IEcsWorldBuilder WithSystem<T>()
        where T : class, ISystem;

    IWorld Build(IServiceProvider gameServices);
}