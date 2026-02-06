using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class PowerUpLifetimeSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        var result = World.CreateQuery()
            .With<PowerUp>()
            .GetResult()
            .Entities;

        foreach (var entity in result)
        {
            ref var powerUp = ref entity.GetComponent<PowerUp>(World);
            powerUp.ElapsedSeconds += deltaTime;
            if (powerUp.ElapsedSeconds >= powerUp.TimeToLiveSeconds)
            {
                World.DestroyEntity(entity);
            }
        }
    }
}
