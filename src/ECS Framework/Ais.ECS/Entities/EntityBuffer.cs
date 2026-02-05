using Ais.ECS.Abstractions.Entities;

namespace Ais.ECS.Entities;

internal sealed class EntityBuffer : IEntityFactory, IEntityRegistry
{
    private readonly Stack<EntityId> _free = [];
    private readonly Lock _sync = new();
    private readonly int[] _versions;

    private int _capacity;

    public EntityBuffer(int initialCapacity = 1024)
    {
        _capacity = initialCapacity;
        _versions = new int[initialCapacity];

        for (var i = initialCapacity - 1; i >= 0; i--)
        {
            var id = new EntityId(i);
            _free.Push(id);
            _versions[id.Value] = 0;
        }
    }

    public IEntity CreateEntity()
    {
        lock (_sync)
        {
            if (!_free.TryPop(out var id))
            {
                Expand();
                id = new EntityId(_capacity - 1);
            }

            _versions[id.Value]++;

            return new Entity(id, _versions[id.Value]);
        }
    }

    public void DestroyEntity(IEntity entity)
    {
        lock (_sync)
        {
            _free.Push(entity.Id);
            _versions[entity.Id.Value]--;
        }
    }

    public ReadOnlySpan<IEntity> GetAllEntities()
    {
        lock (_sync)
        {
            var nextId = _free.Peek();

            if (nextId.Value == 0)
            {
                return [];
            }

            var cursor = 1;
            var entities = new IEntity[nextId.Value];
            for (var i = 0; i < nextId.Value; i++)
            {
                var id = new EntityId(i);
                var version = _versions[id.Value];

                if (version == 0)
                {
                    continue;
                }

                if (cursor + version - 1 > entities.Length)
                {
                    Array.Resize(ref entities, entities.Length + version);
                }

                var currentVersion = version;
                while (currentVersion > 0)
                {
                    var entity = new Entity(id, currentVersion);
                    entities[entity.Id.Value + currentVersion - 1] = entity;

                    currentVersion--;
                    cursor++;
                }
            }

            return entities.AsSpan();
        }
    }

    public ReadOnlySpan<IEntity> GetEntities(EntityId entityId)
    {
        lock (_sync)
        {
            if (_versions[entityId.Value] == 0)
            {
                return [];
            }

            var entities = new IEntity[_versions[entityId.Value]];
            for (var i = 1; i <= _versions[entityId.Value]; i++)
            {
                entities[i - 1] = new Entity(entityId, i);
            }

            return entities.AsSpan();
        }
    }

    private void Expand()
    {
        var newCapacity = _capacity * 2;

        for (var i = _capacity; i < newCapacity; i++)
        {
            var id = new EntityId(i);
            _free.Push(id);
            _versions[id.Value] = 0;
        }

        _capacity = newCapacity;
    }
}
