namespace Ais.ECS.Abstractions.Components;

public interface IComponentRegistry
{
    IReadOnlyList<IComponentStore> ComponentStores { get; }

    bool HasStore(Type type);

    bool HasStore<T>()
        where T : IComponent;

    IComponentStore GetStore(Type type);

    IComponentStore<T> GetStore<T>()
        where T : IComponent;
}
