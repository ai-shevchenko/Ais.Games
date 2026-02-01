using Ais.ECS.Abstractions.Components;
using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Queries;
using Ais.ECS.Abstractions.Systems;

namespace Ais.ECS.Abstractions.Worlds;

public interface IWorld : IEntityRegistry, IEntityFactory, IComponentRegistry, ISystemManager, IQueryBuilder
{
}
