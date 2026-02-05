using Ais.ECS;
using Ais.ECS.Extensions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Systems;

namespace Ais.Games.SnakeGame.Tests;

public class MovementSystemTests
{
    [Fact]
    public void MovementSystem_MovesHeadAndBodyForward()
    {
        var world = new World(new EcsSettings());
        var system = new MovementSystem();
        world.AddSystem(system);

        var head = world.CreateEntity();
        head.AddComponent(world, new SnakeSegment { IsHead = true, Order = 0 });
        head.AddComponent(world, new Position { X = 5, Y = 5 });
        head.AddComponent(world, new Velocity { DirectionX = 1, DirectoinY = 0 });

        var tail = world.CreateEntity();
        tail.AddComponent(world, new SnakeSegment { IsHead = false, Order = 1 });
        tail.AddComponent(world, new Position { X = 4, Y = 5 });

        world.UpdateSystems(0.2f);

        var headPos = head.GetComponent<Position>(world);
        var tailPos = tail.GetComponent<Position>(world);

        Assert.Equal(6, headPos.X);
        Assert.Equal(5, headPos.Y);

        Assert.Equal(5, tailPos.X);
        Assert.Equal(5, tailPos.Y);
    }
}
