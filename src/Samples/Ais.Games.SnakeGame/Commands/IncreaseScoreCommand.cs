using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class IncreaseScoreCommand : ICommand
{
    private const int ScorePoint = 10;

    public required IWorld World { get; init; }

    public void Execute()
    {
        var scoreEntities = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        IEntity scoreEntity;
        Score score;

        if (scoreEntities.Length == 0)
        {
            scoreEntity = World.CreateEntity();
            score = new Score { Value = 0, FruitsEaten = 0, PowerUpsCollected = 0, ScoreMultiplier = 1 };
            scoreEntity.AddComponent(World, score);
        }
        else
        {
            scoreEntity = scoreEntities[0];
            score = scoreEntity.GetComponent<Score>(World);
        }

        var points = ScorePoint * (score.ScoreMultiplier <= 0 ? 1 : score.ScoreMultiplier);
        score.Value += points;
        score.FruitsEaten += 1;

        ref var scoreRef = ref scoreEntity.GetComponent<Score>(World);
        scoreRef = score;
    }

    public void Undo()
    {
    }
}
