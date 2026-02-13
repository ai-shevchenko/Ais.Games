using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

using Microsoft.Extensions.Options;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class MovementSystem : EcsSystem
{
    private const float StepInterval = 0.1f;
    private readonly float _xStepInterval;
    private readonly float _yStepInterval;
    private float _xAccumulator;
    private float _yAccumulator;

    public MovementSystem(IOptions<GameWindowSettings> windowSettings)
    {
        var settings = windowSettings.Value;
        _xStepInterval = StepInterval;
        _yStepInterval = StepInterval * ((float)settings.Width / settings.Height);
    }

    public override void Update(float deltaTime)
    {
        _xAccumulator += deltaTime;
        _yAccumulator += deltaTime;

        var xStep = _xAccumulator >= _xStepInterval;
        var yStep = _yAccumulator >= _yStepInterval;

        if (!xStep && !yStep)
        {
            return;
        }

        if (xStep)
        {
            _xAccumulator -= _xStepInterval;
        }

        if (yStep)
        {
            _yAccumulator -= _yStepInterval;
        }

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
            if (!World.GetStore<SnakeSegment>().Contains(entity))
            {
                continue;
            }

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

        ApplyMovementStep(headEntity, segments, headIndex, headVelocity, xStep, yStep, true);
    }

    private void ApplyMovementStep(
        IEntity headEntity,
        List<(IEntity Entity, SnakeSegment Segment)> segments,
        int headIndex,
        Velocity headVelocity,
        bool xStep,
        bool yStep,
        bool applyGrowth)
    {
        var previousPositions = new Position[segments.Count];
        for (var i = 0; i < segments.Count; i++)
        {
            previousPositions[i] = segments[i].Entity.GetComponent<Position>(World);
        }

        ref var headPos = ref headEntity.GetComponent<Position>(World);
        var oldX = headPos.X;
        var oldY = headPos.Y;

        if (xStep && headVelocity.DirectionX != 0)
        {
            headPos.X += headVelocity.DirectionX;
        }

        if (yStep && headVelocity.DirectionY != 0)
        {
            headPos.Y += headVelocity.DirectionY;
        }

        var headMoved = headPos.X != oldX || headPos.Y != oldY;
        if (!headMoved)
        {
            return;
        }

        for (var i = 1; i < segments.Count; i++)
        {
            ref var pos = ref segments[i].Entity.GetComponent<Position>(World);
            pos = previousPositions[i - 1];
        }

        if (!applyGrowth)
        {
            return;
        }

        if (World.GetStore<GrowthPending>().Contains(headEntity))
        {
            ref var growth = ref headEntity.GetComponent<GrowthPending>(World);
            if (growth.PendingCount > 0)
            {
                var tailIndex = segments.Count - 1;
                var newSegment = World.CreateEntity();

                var newOrder = segments.Count; // следующий порядок

                newSegment.AddComponent(World, new SnakeSegment { IsHead = false, Order = newOrder });
                newSegment.AddComponent(World, previousPositions[tailIndex]);
                newSegment.AddComponent(World, new Sprite { Symbol = 'o', Color = ConsoleColor.DarkGreen });

                growth.PendingCount--;
            }
        }
    }
}
