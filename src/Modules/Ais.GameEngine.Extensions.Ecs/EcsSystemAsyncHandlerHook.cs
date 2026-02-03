using Ais.ECS.Abstractions.Worlds;
using Ais.GameEngine.Hooks.Abstractions;

namespace Ais.GameEngine.Extensions.Ecs;

internal sealed class EcsSystemAsyncHandlerHook : BaseHook, IAsyncInitialize, IAsyncUpdate, IAsyncFixedUpdate,
    IAsyncLateUpdate, IAsyncRender, IAsyncDestroy
{
    private readonly IWorld _world;

    public EcsSystemAsyncHandlerHook(IWorld world)
    {
        _world = world;
    }

    public async Task OnDestroyAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncDestroy>()
            .Select(x => x.OnDestroyAsync(cancellationToken)));
    }

    public async Task FixedUpdateAsync(float fixedDeltaTime, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncFixedUpdate>()
            .Select(x => x.FixedUpdateAsync(fixedDeltaTime, cancellationToken)));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncInitialize>()
            .Select(x => x.InitializeAsync(cancellationToken)));
    }

    public async Task LateUpdateAsync(float deltaTime, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncLateUpdate>()
            .Select(x => x.LateUpdateAsync(deltaTime, cancellationToken)));
    }

    public async Task RenderAsync(float interpolationFactor, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncRender>()
            .Select(x => x.RenderAsync(interpolationFactor, cancellationToken)));
    }

    public async Task UpdateAsync(float deltaTime, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_world.Systems
            .OfType<IAsyncUpdate>()
            .Select(x => x.UpdateAsync(deltaTime, cancellationToken)));
    }
}
