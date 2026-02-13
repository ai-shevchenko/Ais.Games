using Ais.ECS.Extensions;
using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.StateMachine.Abstractions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Events;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class AnomalyDetectionSystem : EcsSystem
{
    private readonly IGameContextAccessor _accessor;
    private readonly IGameLoopEventBus _eventBus;
    private int _lastKnownSnakeLength = -1;

    public AnomalyDetectionSystem(IGameLoopEventBus eventBus, IGameContextAccessor accessor)
    {
        _eventBus = eventBus;
        _accessor = accessor;
    }

    public override void Update(float deltaTime)
    {
        CheckSnakeLength();
        CheckPowerUpDurations();
        CheckSnakePositions();
    }

    private void CheckSnakeLength()
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .GetResult()
            .Entities;

        var currentLength = segments.Length;

        if (currentLength < _lastKnownSnakeLength)
        {
            _ = _eventBus.PublishAsync(new SnakeLengthMismatchEvent
            {
                SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                ExpectedLength = _lastKnownSnakeLength,
                ActualLength = currentLength
            });
        }

        _lastKnownSnakeLength = currentLength;
    }

    private void CheckPowerUpDurations()
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .GetResult()
            .Entities;

        foreach (var segment in segments)
        {
            // Check if segment still has SnakeSegment component
            if (!World.GetStore<SnakeSegment>().Contains(segment))
            {
                continue;
            }

            var seg = segment.GetComponent<SnakeSegment>(World);
            if (!seg.IsHead)
            {
                continue;
            }

            if (World.GetStore<ActivePowerUpEffect>().Contains(segment))
            {
                var effect = segment.GetComponent<ActivePowerUpEffect>(World);
                if (effect.RemainingSeconds > 5f || effect.RemainingSeconds < 0f)
                {
                    _ = _eventBus.PublishAsync(new PowerUpDurationMismatchEvent
                    {
                        SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                        PowerUpType = effect.Type,
                        ExpectedDuration = 5f,
                        ActualDuration = effect.RemainingSeconds
                    });
                }
            }
        }
    }

    private void CheckSnakePositions()
    {
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        var positionMap = new Dictionary<int, Position>();
        foreach (var segment in segments)
        {
            var seg = segment.GetComponent<SnakeSegment>(World);
            var pos = segment.GetComponent<Position>(World);
            positionMap[seg.Order] = pos;
        }

        for (var i = 0; i < positionMap.Count - 1; i++)
        {
            if (!positionMap.ContainsKey(i) || !positionMap.ContainsKey(i + 1))
            {
                _ = _eventBus.PublishAsync(new PositionSyncErrorEvent
                {
                    SourceLoopName = _accessor.CurrentContext?.LoopName ?? "unknown",
                    ExpectedX = 0,
                    ExpectedY = 0,
                    ActualX = 0,
                    ActualY = 0,
                    EntityType = "SnakeSegment"
                });
            }
        }
    }
}
