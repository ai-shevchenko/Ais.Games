using Ais.ECS.Abstractions.Components;
using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Worlds;

namespace Ais.ECS.Extensions;

public static class EcsExtensions
{
    extension(IEntity entity)
    {
        public IEntity AddComponent<T>(IWorld world, T component)
            where T : IComponent
        {
            var store = world.GetStore<T>();
            store.Add(entity, component);

            return entity;
        }

        public IEntity AddComponent<T>(IWorld world)
            where T : IComponent
        {
            var store = world.GetStore<T>();
            var component = Activator.CreateInstance<T>();
            store.Add(entity, component);

            return entity;
        }

        public IEntity AddComponent<T>(IWorld world, Action<T> init)
            where T : IComponent
        {
            var store = world.GetStore<T>();
            var component = Activator.CreateInstance<T>();
            init(component);
            store.Add(entity, component);
            
            return entity;
        }

        public ref T GetComponent<T>(IWorld world)
            where T : IComponent
        {
            return ref world.GetStore<T>().Get(entity);
        }
        
        public IReadOnlyList<IComponent> GetComponents(IWorld world)
        {
            return [.. world.ComponentStores
                .Where(store => store.Contains(entity))
                .Select(store => store.GetBoxed(entity))];
        }
    }
}