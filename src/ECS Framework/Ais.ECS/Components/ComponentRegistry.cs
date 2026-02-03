using System.Collections.Concurrent;
using Ais.ECS.Abstractions.Components;

namespace Ais.ECS.Components;

internal sealed class ComponentRegistry : IComponentRegistry
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<Type, object> _stores = [];

    public ComponentRegistry(int initialCapacity = 1024)
    {
        _capacity = initialCapacity;
    }

    public IReadOnlyList<IComponentStore> ComponentStores => _stores.Values
        .Cast<IComponentStore>()
        .ToList();

    public IComponentStore GetStore(Type type)
    {
        if (!typeof(IComponent).IsAssignableFrom(type))
        {
            throw new ArgumentException($"Type '{type.FullName}' is not component");
        }

        if (!_stores.TryGetValue(type, out var store))
        {
            store = Activator.CreateInstance(typeof(ComponentStore<>).MakeGenericType(type), _capacity)!;
            _stores.TryAdd(type, store);
        }

        return (IComponentStore)store;
    }

    public IComponentStore<T> GetStore<T>()
        where T : IComponent
    {
        var type = typeof(T);
        if (!_stores.TryGetValue(type, out var store))
        {
            store = new ComponentStore<T>(_capacity);
            _stores.TryAdd(type, store);
        }

        return (IComponentStore<T>)store;
    }

    public bool HasStore(Type type)
    {
        if (!typeof(IComponent).IsAssignableFrom(type))
        {
            throw new ArgumentException($"Type '{type.FullName}' is not component");
        }

        return _stores.ContainsKey(type);
    }

    public bool HasStore<T>()
        where T : IComponent
    {
        return _stores.ContainsKey(typeof(T));
    }
}
