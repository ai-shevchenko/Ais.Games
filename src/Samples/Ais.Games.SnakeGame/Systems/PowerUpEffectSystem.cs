using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Commands.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Commands;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Events;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class PowerUpEffectSystem : EcsSystem
{
    private readonly IGameContextAccessor _accessor;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IGameLoopEventBus _eventBus;

    public PowerUpEffectSystem(ICommandExecutor commandExecutor, IGameLoopEventBus eventBus,
        IGameContextAccessor accessor)
    {
        _commandExecutor = commandExecutor;
        _eventBus = eventBus;
        _accessor = accessor;
    }

    public override void Update(float deltaTime)
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segments.Length == 0)
        {
            return;
        }

        IEntity? head = null;
        foreach (var e in segments)
        {
            // Check if segment still has SnakeSegment component
            if (!World.GetStore<SnakeSegment>().Contains(e))
            {
                continue;
            }

            var seg = e.GetComponent<SnakeSegment>(World);
            if (seg.IsHead)
            {
                head = e;
                break;
            }
        }

        if (head is null)
        {
            return;
        }

        if (World.GetStore<ActivePowerUpEffect>().Contains(head))
        {
            ref var effect = ref head.GetComponent<ActivePowerUpEffect>(World);
            effect.RemainingSeconds -= deltaTime;

            if (effect.RemainingSeconds <= 0f)
            {
                PublishPowerUpExpired(effect.Type);
                ResetScoreMultiplier();
                World.GetStore<ActivePowerUpEffect>().Remove(head);
                return;
            }

            if (effect.Type == PowerUpType.DoubleScore)
            {
                EnsureScoreMultiplier(2);
            }
        }
        else
        {
            ResetScoreMultiplier();
        }
    }

    private void EnsureScoreMultiplier(int multiplier)
    {
        _commandExecutor.Execute(new EnsureScoreMultiplierCommand { World = World, Multiplier = multiplier });
    }

    private void ResetScoreMultiplier()
    {
        _commandExecutor.Execute(new EnsureScoreMultiplierCommand { World = World, Multiplier = 1 });
    }

    private void PublishPowerUpExpired(PowerUpType powerUpType)
    {
        _ = _eventBus.PublishAsync(new PowerUpExpiredEvent
        {
            SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown", PowerUpType = powerUpType
        });
    }
}
