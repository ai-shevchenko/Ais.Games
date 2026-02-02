using Ais.Commons.Commands.Abstractions;
using Ais.ECS.Abstractions.Worlds;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class RemoveFoodCommand : ICommand
{
    public required IWorld World { get; init; }

    public void Execute()
    {
        var entities = World.CreateQuery()
            .With<Food>()
            .GetResult()
            .Entities;

        foreach (var entity in entities)
        {
            World.DestroyEntity(entity);
        }
    }

    public void Undo()
    {
    }
}
