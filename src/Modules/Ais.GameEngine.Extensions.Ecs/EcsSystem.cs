using System.Diagnostics.CodeAnalysis;
using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Extensions.Ecs;

public abstract class EcsSystem : BaseHook, ISystem, IUpdate
{
    [NotNull] protected IWorld? World;

    public virtual void Initialize(SystemContext context)
    {
        World = context.World;
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void Shutdown()
    {
    }
}
