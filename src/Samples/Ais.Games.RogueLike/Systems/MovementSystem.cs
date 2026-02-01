using Ais.ECS.Abstractions;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class MovementSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        var result = World.CreateQuery()
            .With<Position>()
            .With<Velocity>()
            .With<SnakeSegment>()
            .GetResult();

        foreach (var entity in result.Entities)
        {
            var position = entity.GetComponent<Position>(World);
            var velocity = entity.GetComponent<Velocity>(World);
            var segment = entity.GetComponent<SnakeSegment>(World);


        }
    }
}
