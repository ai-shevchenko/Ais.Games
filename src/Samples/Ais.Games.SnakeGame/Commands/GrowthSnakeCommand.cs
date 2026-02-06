using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class GrowthSnakeCommand : ICommand
{
    public required IWorld World { get; init; }

    public void Execute()
    {
        var segmentsSpan = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segmentsSpan.Length == 0)
        {
            return;
        }

        var segmentsList = new List<(IEntity Entity, SnakeSegment Segment, Position Position)>(segmentsSpan.Length);
        foreach (var e in segmentsSpan)
        {
            var seg = e.GetComponent<SnakeSegment>(World);
            var pos = e.GetComponent<Position>(World);
            segmentsList.Add((e, seg, pos));
        }

        segmentsList.Sort((a, b) => a.Segment.Order.CompareTo(b.Segment.Order));

        var tail = segmentsList[^1];
        var newSegment = World.CreateEntity();
        newSegment.AddComponent(World, new SnakeSegment { IsHead = false, Order = tail.Segment.Order + 1 });
        newSegment.AddComponent(World, tail.Position);
        newSegment.AddComponent(World, new Sprite { Symbol = 'o', Color = ConsoleColor.DarkGreen });
    }

    public void Undo()
    {
    }
}
