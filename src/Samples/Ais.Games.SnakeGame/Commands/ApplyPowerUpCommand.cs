using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Abstractions.Worlds;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Commands;

internal sealed class ApplyPowerUpCommand : ICommand
{
    public required IWorld World { get; init; }

    public required IEntity Head { get; init; }
    public required IEntity PowerUp { get; init; }

    public void Execute()
    {
        ref var powerUp = ref PowerUp.GetComponent<PowerUp>(World);

        var scoreEntities = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        if (scoreEntities.Length > 0)
        {
            ref var score = ref scoreEntities[0].GetComponent<Score>(World);
            score.PowerUpsCollected += 1;
        }

        var effectStore = World.GetStore<ActivePowerUpEffect>();
        if (!effectStore.Contains(Head))
        {
            Head.AddComponent(World, new ActivePowerUpEffect { Type = powerUp.Type, RemainingSeconds = 5f });
        }
        else
        {
            ref var effect = ref Head.GetComponent<ActivePowerUpEffect>(World);
            effect.Type = powerUp.Type;
            effect.RemainingSeconds = 5f;
        }

        World.DestroyEntity(PowerUp);
    }

    public void Undo()
    {
    }
}
