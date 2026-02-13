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

        foreach (var segment in segmentsSpan)
        {
            if (!World.GetStore<SnakeSegment>().Contains(segment))
            {
                continue;
            }

            var seg = segment.GetComponent<SnakeSegment>(World);
            if (seg.IsHead)
            {
                if (World.GetStore<GrowthPending>().Contains(segment))
                {
                    ref var growth = ref segment.GetComponent<GrowthPending>(World);
                    growth.PendingCount++;
                }
                else
                {
                    segment.AddComponent(World, new GrowthPending { PendingCount = 1 });
                }

                break;
            }
        }
    }

    public void Undo()
    {
    }
}
