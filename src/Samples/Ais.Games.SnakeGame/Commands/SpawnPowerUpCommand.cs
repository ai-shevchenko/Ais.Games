using System;
using System.Runtime;

using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class SpawnPowerUpCommand : ICommand
{
    private readonly Random _random = Random.Shared;

    public required IWorld World { get; init; }
    public required GameWindowSettings WindowSettings { get; init; }

    public void Execute()
    {
        var entities = World.CreateQuery()
            .With<Position>()
            .GetResult()
            .Entities;

        var (newX, newY) = (0, 0);

        while (true)
        {
            newX = _random.Next(1, WindowSettings.Width + 1);
            newY = _random.Next(1, WindowSettings.Height + 1);

            var occupied = false;
            foreach (var entity in entities)
            {
                var pos = entity.GetComponent<Position>(World);
                if (pos.X == newX && pos.Y == newY)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
            {
                break;
            }
        }

        var type = _random.Next(0, 2) == 0 ? PowerUpType.SpeedBoost : PowerUpType.DoubleScore;
        var powerUp = World.CreateEntity();
        powerUp.AddComponent(World, new Position { X = newX, Y = newY });
        powerUp.AddComponent(World, new PowerUp { Type = type, TimeToLiveSeconds = 8f, ElapsedSeconds = 0f });

        var sprite = type switch
        {
            PowerUpType.SpeedBoost => new Sprite { Symbol = 'S', Color = ConsoleColor.Cyan },
            PowerUpType.DoubleScore => new Sprite { Symbol = '$', Color = ConsoleColor.Yellow },
            _ => new Sprite { Symbol = '?', Color = ConsoleColor.White }
        };

        powerUp.AddComponent(World, sprite);
    }

    public void Undo()
    {
    }
}
