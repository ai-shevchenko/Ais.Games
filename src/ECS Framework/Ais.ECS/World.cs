using Ais.ECS.Abstractions.Components;
using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Queries;
using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Components;
using Ais.ECS.Entities;
using Ais.ECS.Queries;
using IQueryProvider = Ais.ECS.Abstractions.Queries.IQueryProvider;

namespace Ais.ECS;

public sealed class World : IWorld
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly ISystemManager _systemManager;
    private readonly IQueryProvider _queryProvider;

    public World(EcsSettings settings)
    {
        var entityBuffer = new EntityBuffer(settings.EntityBufferSize);
        _entityFactory = entityBuffer;
        _entityRegistry = entityBuffer;
        _componentRegistry = new ComponentRegistry(settings.ComponentBufferSize);
        _systemManager = new SystemManager(this);
        _queryProvider =  new QueryProvider(this);
    }

    public IReadOnlyList<ISystem> Systems => _systemManager.Systems;
    
    public IReadOnlyList<IComponentStore> ComponentStores => _componentRegistry.ComponentStores;
    
    public ReadOnlySpan<IEntity> GetAllEntities()
    {
        return _entityRegistry.GetAllEntities();
    }

    public ReadOnlySpan<IEntity> GetEntities(EntityId entityId)
    {
        return _entityRegistry.GetEntities(entityId);
    }

    public IEntity CreateEntity()
    {
        return _entityFactory.CreateEntity();
    }

    public void DestroyEntity(IEntity entity)
    {   
        _entityFactory.DestroyEntity(entity);
    }
    
    public bool HasStore(Type type)
    {
        return _componentRegistry.HasStore(type);
    }

    public bool HasStore<T>() 
        where T : IComponent
    {
        return _componentRegistry.HasStore<T>();
    }

    public IComponentStore GetStore(Type type)
    {
        return _componentRegistry.GetStore(type);
    }

    public IComponentStore<T> GetStore<T>() where T : IComponent
    {
        return _componentRegistry.GetStore<T>();
    }
    
    public void AddSystem(ISystem system)
    {
        _systemManager.AddSystem(system);
    }

    public void RemoveSystem(ISystem system)
    {
        _systemManager.RemoveSystem(system);
    }

    public void UpdateSystems(float deltaTime)
    {
        _systemManager.UpdateSystems(deltaTime);
    }

    public TSystem GetSystem<TSystem>() 
        where TSystem : class, ISystem
    {
        return _systemManager.GetSystem<TSystem>();
    }
    
    public IQuery CreateQuery()
    {
        return _queryProvider.QueryBuilder.CreateQuery();
    }
}