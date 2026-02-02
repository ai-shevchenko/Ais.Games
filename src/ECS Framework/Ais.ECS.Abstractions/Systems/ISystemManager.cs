namespace Ais.ECS.Abstractions.Systems;

public interface ISystemManager
{
    IReadOnlyList<ISystem> Systems { get; }

    void AddSystem(ISystem system);
    void RemoveSystem(ISystem system);
    void UpdateSystems(float deltaTime);

    TSystem GetSystem<TSystem>()
        where TSystem : class, ISystem;
}
