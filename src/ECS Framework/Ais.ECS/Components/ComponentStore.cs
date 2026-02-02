using Ais.ECS.Abstractions.Components;
using Ais.ECS.Abstractions.Entities;

namespace Ais.ECS.Components;

internal sealed class ComponentStore<T> : IComponentStore<T>
    where T : IComponent
{
    private readonly Lock _sync = new();
    private int _capacity;
    private T[] _components;
    private int[] _entityToIndex;
    private int[] _indexToEntity;

    public ComponentStore(int initialCapacity = 1024)
    {
        _capacity = initialCapacity;
        _components = new T[initialCapacity];
        _entityToIndex = new int[initialCapacity];
        _indexToEntity = new int[initialCapacity];
        Array.Fill(_entityToIndex, -1);
    }

    public Type ComponentType => typeof(T);

    public int Count { get; private set; }

    public IComponent GetBoxed(IEntity entity)
    {
        return Get(entity);
    }

    public ref T Add(IEntity entity, in T component)
    {
        lock (_sync)
        {
            if (Count >= _capacity)
            {
                Expand();
            }

            var index = Count++;
            _entityToIndex[entity.Id.Value] = index;
            _indexToEntity[index] = entity.Id.Value;
            _components[index] = component;

            return ref _components[index];
        }
    }

    public bool Contains(IEntity entity)
    {
        lock (_sync)
        {
            return entity.Id.Value < _capacity
                   && _entityToIndex[entity.Id.Value] >= 0;
        }
    }

    public ref T Get(IEntity entity)
    {
        lock (_sync)
        {
            var index = _entityToIndex[entity.Id.Value];

            if (index < 0 || index >= Count)
            {
                throw new InvalidOperationException($"Component {typeof(T).Name} not found");
            }

            return ref _components[index];
        }
    }

    public bool Remove(IEntity entity)
    {
        lock (_sync)
        {
            var index = _entityToIndex[entity.Id.Value];
            if (index < 0)
            {
                return false;
            }

            var lastEntityId = _indexToEntity[Count - 1];
            if (index != Count - 1)
            {
                _components[index] = _components[Count - 1];
                _indexToEntity[index] = lastEntityId;
                _entityToIndex[lastEntityId] = index;
            }

            _entityToIndex[entity.Id.Value] = -1;
            Count--;

            return true;
        }
    }

    private void Expand()
    {
        var newCapacity = _capacity * 2;
        Array.Resize(ref _components, newCapacity);
        Array.Resize(ref _entityToIndex, newCapacity);
        Array.Resize(ref _indexToEntity, newCapacity);

        for (var i = _capacity; i < newCapacity; i++)
        {
            _entityToIndex[i] = -1;
        }

        _capacity = newCapacity;
    }
}
