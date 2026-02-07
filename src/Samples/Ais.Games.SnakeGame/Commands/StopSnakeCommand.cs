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
            .With<PlayerControlled>()
            .With<Velocity>()
            .GetResult()
            .Entities;

        foreach (var entity in heads)
        {
            ref var control = ref entity.GetComponent<PlayerControlled>(World);
            control.Available = false;

            ref var velocity = ref entity.GetComponent<Velocity>(World);
            velocity.DirectionX = 0;
            velocity.DirectoinY = 0;
        }
    }

    public void Undo()
    {
    }
}
