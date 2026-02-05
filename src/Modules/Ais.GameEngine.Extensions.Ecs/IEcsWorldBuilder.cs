using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;

using Microsoft.Extensions.DependencyInjection;

namespace Ais.GameEngine.Extensions.Ecs;

public interface IEcsWorldBuilder
{
    IServiceCollection Services { get; }

    IEcsWorldBuilder WithSystem<T>()
        where T : class, ISystem;

    IEcsWorldBuilder WithWorldSetup(Action<IServiceProvider, IWorld> configure);

    IWorld Build(IServiceProvider serviceProvider);
}
