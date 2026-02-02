using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;

namespace Ais.ECS;

public sealed class SystemManager : ISystemManager
{
    private readonly SystemContext _context;
    private readonly Lock _sync = new();
    private readonly List<ISystem> _systems = [];
    private readonly Dictionary<Type, ISystem> _typedSystems = [];

    public SystemManager(IWorld world)
    {
        _context = new SystemContext(world);
    }

    public IReadOnlyList<ISystem> Systems => _systems;

    public void AddSystem(ISystem system)
    {
        lock (_sync)
        {
            _systems.Add(system);
            _typedSystems[system.GetType()] = system;
            system.Initialize(_context);
        }
    }

    public void RemoveSystem(ISystem system)
    {
        lock (_sync)
        {
            _systems.Remove(system);
            _typedSystems.Remove(system.GetType());
            system.Shutdown();
        }
    }

    public void UpdateSystems(float deltaTime)
    {
        lock (_sync)
        {
            foreach (var system in _systems)
            {
                system.Update(deltaTime);
            }
        }
    }

    public TSystem GetSystem<TSystem>()
        where TSystem : class, ISystem
    {
        lock (_sync)
        {
            if (!_typedSystems.TryGetValue(typeof(TSystem), out var system))
            {
                throw new InvalidOperationException($"The system {typeof(TSystem).Name} does not exist.");
            }

            return (TSystem)system;
        }
    }
}
