using Ais.Commons.Commands.Abstractions;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class SpawnFoodCommand : ICommand
{
    public required IWorld World { get; init; }
    public required GameWindowSettings WindowSettings { get; init; }

    public void Execute()
    {
        var entities = World.CreateQuery()
            .With<Position>()
            .GetResult()
            .Entities;

        var (newX, newY) = (0, 0);

        var random = Random.Shared;

        var generate = false;
        while (true)
        {
            newX = random.Next(1, WindowSettings.Width - 1);
            newY = random.Next(1, WindowSettings.Height - 1);

            foreach (var entity in entities)
            {
                var entityPos = entity.GetComponent<Position>(World);
                if (entityPos.X != newX || entityPos.Y != newY)
                {
                    continue;
                }

                generate = true;
                break;
            }

            if (!generate)
            {
                break;
            }
        }

        var food = World.CreateEntity();
        food.AddComponent(World, new Position { X = newX, Y = newY });
        food.AddComponent(World, new Sprite { Symbol = '@', Color = ConsoleColor.Red });
    }

    public void Undo()
    {
    }
}
