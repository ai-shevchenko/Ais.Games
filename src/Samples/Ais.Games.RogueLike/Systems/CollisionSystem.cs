using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class CollisionSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        foreach (var entity in segments)
        {
            var position = entity.GetComponent<Position>(World);
        }
    }
}
