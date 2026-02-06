using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class EnsureScoreMultiplierCommand : ICommand
{
    public required IWorld World { get; init; }

    public required int Multiplier { get; init; }

    public void Execute()
    {
        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        if (scores.Length == 0)
        {
            return;
        }

        foreach (var entity in scores)
        {
            ref var score = ref entity.GetComponent<Score>(World);
            score.ScoreMultiplier = Multiplier;
        }
    }

    public void Undo()
    {
    }
}
