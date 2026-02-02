using Ais.ECS.Abstractions.Worlds;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Extensions.Ecs;

internal sealed class EcsSystemHandlerHook : BaseHook, IInitialize, IUpdate, IFixedUpdate, ILateUpdate, IRender,
    IDestroy
{
    private readonly IWorld _world;

    public EcsSystemHandlerHook(IWorld world)
    {
        _world = world;
    }

    public void OnDestroy()
    {
        foreach (var hook in _world.Systems.OfType<IDestroy>())
        {
            hook.OnDestroy();
        }
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        foreach (var hook in _world.Systems.OfType<IFixedUpdate>())
        {
            hook.FixedUpdate(fixedDeltaTime);
        }
    }

    public void Initialize()
    {
        foreach (var hook in _world.Systems.OfType<IInitialize>())
        {
            hook.Initialize();
        }
    }

    public void LateUpdate(float deltaTime)
    {
        foreach (var hook in _world.Systems.OfType<ILateUpdate>())
        {
            hook.LateUpdate(deltaTime);
        }
    }

    public void Render(float alpha)
    {
        foreach (var hook in _world.Systems.OfType<IRender>())
        {
            hook.Render(alpha);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var hook in _world.Systems)
        {
            hook.Update(deltaTime);
        }
    }
}
