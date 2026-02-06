using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.GameEngine.Extensions.SignalBus.Abstractions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Signals;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class MovementSystem : EcsSystem
{
    private const float StepInterval = 0.1f;

    private readonly ISignalBus _signalBus;

    public MovementSystem(ISignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private float _accumulator;

    public override void Update(float deltaTime)
    {
        _accumulator += deltaTime;
        if (_accumulator < StepInterval)
        {
            return;
        }

        _accumulator = 0f;

        var result = World.CreateQuery()
            .With<Position>()
            .With<SnakeSegment>()
            .GetResult();

        var entities = result.Entities;
        if (entities.Length == 0)
        {
            return;
        }

        var segments = new List<(IEntity Entity, SnakeSegment Segment)>(entities.Length);
        foreach (var entity in entities)
        {
            var segment = entity.GetComponent<SnakeSegment>(World);
            segments.Add((entity, segment));
        }

        if (segments.Count == 0)
        {
            return;
        }

        segments.Sort((a, b) => a.Segment.Order.CompareTo(b.Segment.Order));

        var headIndex = -1;
        for (var i = 0; i < segments.Count; i++)
        {
            if (segments[i].Segment.IsHead)
            {
                headIndex = i;
                break;
            }
        }

        if (headIndex < 0)
        {
            return;
        }

        var headEntity = segments[headIndex].Entity;

        if (!World.GetStore<Velocity>().Contains(headEntity))
        {
            return;
        }

        var headVelocity = headEntity.GetComponent<Velocity>(World);

        if (World.GetStore<ActivePowerUpEffect>().Contains(headEntity))
        {
            ref var effect = ref headEntity.GetComponent<ActivePowerUpEffect>(World);
            if (effect.Type == PowerUpType.SpeedBoost)
            {
                ApplyMovementStep(headEntity, segments, headIndex, headVelocity);
            }
        }

        ApplyMovementStep(headEntity, segments, headIndex, headVelocity);
    }

    private void ApplyMovementStep(
        IEntity headEntity,
        List<(IEntity Entity, SnakeSegment Segment)> segments,
        int headIndex,
        Velocity headVelocity)
    {
        var previousPositions = new Position[segments.Count];
        for (var i = 0; i < segments.Count; i++)
        {
            previousPositions[i] = segments[i].Entity.GetComponent<Position>(World);
        }

        ref var headPos = ref headEntity.GetComponent<Position>(World);
        var oldPos = new Position { X = headPos.X, Y = headPos.Y };
        headPos.X += headVelocity.DirectionX;
        headPos.Y += headVelocity.DirectoinY;

        _signalBus.Publish(new SnakeMoveSignal(oldPos, headPos));

        for (var i = 0; i < segments.Count; i++)
        {
            if (i == headIndex)
            {
                continue;
            }

            var targetIndex = i < headIndex ? i + 1 : i - 1;
            ref var pos = ref segments[i].Entity.GetComponent<Position>(World);
            pos = previousPositions[targetIndex];
        }
    }
}
