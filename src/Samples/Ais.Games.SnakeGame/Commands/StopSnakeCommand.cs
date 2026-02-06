using System;
using System.Collections.Generic;
using System.Text;

using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class StopSnakeCommand : ICommand
{
    public required IWorld World { get; init; }

    public void Execute()
    {
        var heads = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Velocity>()
            .GetResult()
            .Entities;

        foreach (var entity in heads)
        {
            var segment = entity.GetComponent<SnakeSegment>(World);
            if (segment.IsHead)
            {
                ref var velocity = ref entity.GetComponent<Velocity>(World);
                velocity.DirectionX = 0;
                velocity.DirectoinY = 0;

                break;
            }
        }
    }

    public void Undo()
    {
    }
}
