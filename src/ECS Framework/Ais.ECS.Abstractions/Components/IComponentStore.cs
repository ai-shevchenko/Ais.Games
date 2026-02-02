using Ais.ECS.Abstractions.Entities;

namespace Ais.ECS.Abstractions.Components;

public interface IComponentStore
{
    Type ComponentType { get; }

    int Count { get; }

    IComponent GetBoxed(IEntity entity);

    bool Contains(IEntity entity);
    bool Remove(IEntity entity);
}

public interface IComponentStore<T> : IComponentStore
    where T : IComponent
{
    ref T Get(IEntity entity);
    ref T Add(IEntity entity, in T component);
}
