using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

internal sealed class MovementSystem : EcsSystem
{
    private float _accumulator;
    private const float StepInterval = 0.1f;

    public override void Update(float deltaTime)
    {
        _accumulator += deltaTime;
        if (_accumulator < StepInterval)
        {
            return;
        }

        _accumulator = 0f;

        // Находим все сегменты змейки
        var result = World.CreateQuery()
            .With<Position>()
            .With<SnakeSegment>()
            .GetResult();

        var entities = result.Entities;
        if (entities.Length == 0)
        {
            return;
        }

        // Собираем список сегментов с их порядком
        var segments = new List<(Ais.ECS.Abstractions.Entities.IEntity Entity, SnakeSegment Segment)>(entities.Length);
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

        // Голова — первый элемент с IsHead
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

        // Для головы должна быть скорость
        if (!World.GetStore<Velocity>().Contains(headEntity))
        {
            return;
        }

        var headVelocity = headEntity.GetComponent<Velocity>(World);

        // Если на голове активен паверап ускорения — двигаемся чаще
        if (World.GetStore<ActivePowerUpEffect>().Contains(headEntity))
        {
            ref var effect = ref headEntity.GetComponent<ActivePowerUpEffect>(World);
            if (effect.Type == PowerUpType.SpeedBoost)
            {
                // Дополнительный шаг движения
                ApplyMovementStep(headEntity, segments, headIndex, headVelocity);
            }
        }

        // Сохраняем предыдущие позиции по порядку сегментов
        ApplyMovementStep(headEntity, segments, headIndex, headVelocity);
    }

    private void ApplyMovementStep(
        Ais.ECS.Abstractions.Entities.IEntity headEntity,
        List<(Ais.ECS.Abstractions.Entities.IEntity Entity, SnakeSegment Segment)> segments,
        int headIndex,
        Velocity headVelocity)
    {
        var previousPositions = new Position[segments.Count];
        for (var i = 0; i < segments.Count; i++)
        {
            previousPositions[i] = segments[i].Entity.GetComponent<Position>(World);
        }

        // Двигаем голову
        ref var headPos = ref headEntity.GetComponent<Position>(World);
        headPos.X += headVelocity.DirectionX;
        headPos.Y += headVelocity.DirectoinY;

        // Остальные сегменты "догоняют" предыдущий сегмент
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
